import { describe, it, expect, vi } from "vitest";
import { shallowMount } from "@vue/test-utils";
import ConsultReservationPage from "../ConsultReservationPage.vue";

describe("ConsultReservationPage.vue - unit tests de métodos", () => {
  const mountComponent = () => {
    const pushMock = vi.fn();

    const wrapper = shallowMount(ConsultReservationPage, {
      global: {
        mocks: {
          $router: {
            push: pushMock
          }
        },
        stubs: {
          HeaderLogoNoAdmin: true
        }
      }
    });

    return {
      wrapper,
      pushMock
    };
  };

  it("inicializa los datos del formulario vacíos", () => {
    // Arrange
    const { wrapper } = mountComponent();

    // Act
    const reservationCode = wrapper.vm.reservationCode;
    const passengerName = wrapper.vm.passengerName;
    const errorMessage = wrapper.vm.errorMessage;

    // Assert
    expect(reservationCode).toBe("");
    expect(passengerName).toBe("");
    expect(errorMessage).toBe("");
  });

  it("asigna mensaje de error si reservationCode y passengerName están vacíos", () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe(
      "Debe ingresar el número de reserva y el nombre del pasajero."
    );
    expect(pushMock).not.toHaveBeenCalled();
  });

  it("asigna mensaje de error si solo falta passengerName", async () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    await wrapper.setData({
      reservationCode: "TXN-d30ae4ba",
      passengerName: ""
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe(
      "Debe ingresar el número de reserva y el nombre del pasajero."
    );
    expect(pushMock).not.toHaveBeenCalled();
  });

  it("asigna mensaje de error si solo falta reservationCode", async () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    await wrapper.setData({
      reservationCode: "",
      passengerName: "Obando Vásquez"
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe(
      "Debe ingresar el número de reserva y el nombre del pasajero."
    );
    expect(pushMock).not.toHaveBeenCalled();
  });

  it("rechaza valores que solo contienen espacios en blanco", async () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    await wrapper.setData({
      reservationCode: "   ",
      passengerName: "   "
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe(
      "Debe ingresar el número de reserva y el nombre del pasajero."
    );
    expect(pushMock).not.toHaveBeenCalled();
  });

  it("redirige a ReservationDetails si los datos son válidos", async () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    await wrapper.setData({
      reservationCode: "TXN-d30ae4ba",
      passengerName: "Obando Vásquez"
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe("");
    expect(pushMock).toHaveBeenCalledTimes(1);
    expect(pushMock).toHaveBeenCalledWith({
      name: "ReservationDetails",
      query: {
        reservationCode: "TXN-d30ae4ba",
        passengerName: "Obando Vásquez"
      }
    });
  });

  it("limpia un error previo antes de validar nuevamente", async () => {
    // Arrange
    const { wrapper, pushMock } = mountComponent();

    await wrapper.setData({
      reservationCode: "",
      passengerName: "",
      errorMessage: "Error anterior"
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe(
      "Debe ingresar el número de reserva y el nombre del pasajero."
    );
    expect(pushMock).not.toHaveBeenCalled();

    // Arrange
    await wrapper.setData({
      reservationCode: "TXN-d30ae4ba",
      passengerName: "Obando Vásquez"
    });

    // Act
    wrapper.vm.goToReservationDetails();

    // Assert
    expect(wrapper.vm.errorMessage).toBe("");
    expect(pushMock).toHaveBeenCalledWith({
      name: "ReservationDetails",
      query: {
        reservationCode: "TXN-d30ae4ba",
        passengerName: "Obando Vásquez"
      }
    });
  });
});