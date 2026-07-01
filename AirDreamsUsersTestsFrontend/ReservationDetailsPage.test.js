import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { shallowMount } from "@vue/test-utils";
import ReservationDetailsPage from "../Sprint_1/AirDreams/frontend/src/views/ReservationDetailsPage.vue";

describe("ReservationDetailsPage.vue - unit tests de métodos", () => {
  const ReservationDetailsPageWithoutMounted = {
    ...ReservationDetailsPage,
    mounted() {}
  };

  const mountComponent = (query = {}) => {
    const wrapper = shallowMount(ReservationDetailsPageWithoutMounted, {
      global: {
        mocks: {
          $route: {
            query
          },
          $router: {
            push: vi.fn()
          }
        },
        stubs: {
          HeaderLogoNoAdmin: true,
          ReservationHero: true,
          ReservationTicketCard: true,
          ReservationOptionsPanel: true
        }
      }
    });

    return { wrapper };
  };

  beforeEach(() => {
    vi.clearAllMocks();

    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({
        passengers: [],
        flights: []
      })
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("inicializa correctamente los datos de la reserva", () => {
    const { wrapper } = mountComponent();

    const reservation = wrapper.vm.reservation;

    expect(wrapper.vm.errorMessage).toBe("");
    expect(reservation.reservationCode).toBe("");
    expect(reservation.destinationCity).toBe("");
    expect(reservation.destinationCode).toBe("");
    expect(reservation.daysLeft).toBe(0);
    expect(reservation.passengerCount).toBe(0);
    expect(reservation.status).toBe("Confirmada");
    expect(reservation.isCancelled).toBe(false);
    expect(reservation.purchase.purchaseType).toBe("direct");
    expect(reservation.tickets).toEqual([]);
  });

  it("mapea correctamente una reserva directa", () => {
    const { wrapper } = mountComponent();

    const reservationCode = "TXN-d30ae4ba";

    const data = {
      passengers: [
        {
          idPassenger: 1,
          passengerName: "Juan Pérez"
        }
      ],
      flights: [
        {
          flightNumber: "AD123",
          originCode: "SJO",
          destinationCode: "MIA",
          originCity: "San José",
          destinationCity: "Miami",
          originCountry: "Costa Rica",
          destinationCountry: "Estados Unidos",
          departureDate: "2026-07-01T10:00:00",
          aircraftModel: "AIRBUS-2026"
        }
      ]
    };

    const result = wrapper.vm.mapReservationDetails(reservationCode, data);

    expect(result.reservationCode).toBe("TXN-d30ae4ba");

    expect(result.destinationCity).toBe("Miami");
    expect(result.destinationCode).toBe("MIA");
    expect(result.passengerCount).toBe(1);
    expect(result.status).toBe("Confirmada");
    expect(result.isCancelled).toBe(false);

    expect(result.purchase.purchaseType).toBe("direct");
    expect(result.purchase.firstIsAirDreams).toBe(true);
    expect(result.purchase.secondIsAirDreams).toBe(true);

    expect(result.purchase.flightCode).toBe("AD123");
    expect(result.purchase.firstFlightCode).toBe("AD123");
    expect(result.purchase.secondFlightCode).toBe("");

    expect(result.purchase.origin).toBe("SJO");
    expect(result.purchase.layover).toBe("");
    expect(result.purchase.destination).toBe("MIA");

    expect(result.purchase.originName).toBe("San José, Costa Rica");
    expect(result.purchase.layoverName).toBe("");
    expect(result.purchase.destinationName).toBe("Miami, Estados Unidos");

    expect(result.purchase.firstAircraft).toBe("AIRBUS-2026");
    expect(result.purchase.secondAircraft).toBe("");

    expect(result.tickets).toHaveLength(1);
    expect(result.tickets[0].id).toBe("TXN-d30ae4ba");
    expect(result.tickets[0].seatNumber).toBeUndefined();
    expect(result.tickets[0].classType).toBe("Turista");

    expect(result.tickets[0].passengers).toHaveLength(1);
    expect(result.tickets[0].passengers[0].id).toBe(1);
    expect(result.tickets[0].passengers[0].name).toBe("Juan Pérez");
  });

  it("mapea correctamente una reserva con escala", () => {
    const { wrapper } = mountComponent();

    const reservationCode = "TXN-escala123";

    const data = {
      passengers: [
        {
          idPassenger: 1,
          passengerName: "Ana Obando"
        },
        {
          idPassenger: 2,
          passengerName: "Carlos Mora"
        }
      ],
      flights: [
        {
          flightNumber: "AD111",
          originCode: "SJO",
          destinationCode: "MIA",
          originCity: "San José",
          destinationCity: "Miami",
          originCountry: "Costa Rica",
          destinationCountry: "Estados Unidos",
          departureDate: "2026-07-01T08:00:00",
          aircraftModel: "AIRBUS-2026"
        },
        {
          flightNumber: "AD222",
          originCode: "MIA",
          destinationCode: "MAD",
          originCity: "Miami",
          destinationCity: "Madrid",
          originCountry: "Estados Unidos",
          destinationCountry: "España",
          departureDate: "2026-07-01T15:00:00",
          aircraftModel: "BOEING-2026"
        }
      ]
    };

    const result = wrapper.vm.mapReservationDetails(reservationCode, data);

    expect(result.reservationCode).toBe("TXN-escala123");

    expect(result.destinationCity).toBe("Madrid");
    expect(result.destinationCode).toBe("MAD");
    expect(result.passengerCount).toBe(2);

    expect(result.purchase.purchaseType).toBe("layover");
    expect(result.purchase.firstIsAirDreams).toBe(true);
    expect(result.purchase.secondIsAirDreams).toBe(true);

    expect(result.purchase.flightCode).toBe("AD111 / AD222");
    expect(result.purchase.firstFlightCode).toBe("AD111");
    expect(result.purchase.secondFlightCode).toBe("AD222");

    expect(result.purchase.origin).toBe("SJO");
    expect(result.purchase.layover).toBe("MIA");
    expect(result.purchase.destination).toBe("MAD");

    expect(result.purchase.originName).toBe("San José, Costa Rica");
    expect(result.purchase.layoverName).toBe("Miami, Estados Unidos");
    expect(result.purchase.destinationName).toBe("Madrid, España");

    expect(result.purchase.firstAircraft).toBe("AIRBUS-2026");
    expect(result.purchase.secondAircraft).toBe("BOEING-2026");

    expect(result.tickets).toHaveLength(1);
    expect(result.tickets[0].id).toBe("TXN-escala123");
    expect(result.tickets[0].seatNumber).toBeUndefined();
    expect(result.tickets[0].classType).toBe("Turista");

    expect(result.tickets[0].passengers).toHaveLength(2);
    expect(result.tickets[0].passengers[0].id).toBe(1);
    expect(result.tickets[0].passengers[0].name).toBe("Ana Obando");
    expect(result.tickets[0].passengers[1].id).toBe(2);
    expect(result.tickets[0].passengers[1].name).toBe("Carlos Mora");
  });

  it("carga los detalles de la reserva desde el backend", async () => {
    const { wrapper } = mountComponent({
      reservationCode: "TXN-d30ae4ba"
    });

    const backendResponse = {
      passengers: [
        {
          idPassenger: 1,
          passengerName: "Ana Obando"
        }
      ],
      flights: [
        {
          flightNumber: "AD123",
          originCode: "SJO",
          destinationCode: "MIA",
          originCity: "San José",
          destinationCity: "Miami",
          originCountry: "Costa Rica",
          destinationCountry: "Estados Unidos",
          departureDate: "2026-07-01T10:00:00",
          aircraftModel: "AIRBUS-2026"
        }
      ]
    };

    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => backendResponse
    });

    await wrapper.vm.loadReservationDetails();

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(global.fetch).toHaveBeenCalledWith(
      expect.stringContaining("/Reservation/TXN-d30ae4ba")
    );

    expect(wrapper.vm.errorMessage).toBe("");
    expect(wrapper.vm.reservation.reservationCode).toBe("TXN-d30ae4ba");
    expect(wrapper.vm.reservation.destinationCity).toBe("Miami");
    expect(wrapper.vm.reservation.destinationCode).toBe("MIA");
    expect(wrapper.vm.reservation.passengerCount).toBe(1);
    expect(wrapper.vm.reservation.purchase.flightCode).toBe("AD123");

    expect(wrapper.vm.reservation.tickets).toHaveLength(1);
    expect(wrapper.vm.reservation.tickets[0].id).toBe("TXN-d30ae4ba");
    expect(wrapper.vm.reservation.tickets[0].passengers).toHaveLength(1);
    expect(wrapper.vm.reservation.tickets[0].passengers[0].id).toBe(1);
    expect(wrapper.vm.reservation.tickets[0].passengers[0].name).toBe("Ana Obando");
  });

  it("muestra error si no se recibe reservationCode", async () => {
    const { wrapper } = mountComponent({});

    await wrapper.vm.loadReservationDetails();

    expect(wrapper.vm.errorMessage).toBe("No se recibió el código de reserva.");
    expect(global.fetch).not.toHaveBeenCalled();
  });

  it("muestra error si el backend responde con error", async () => {
    const consoleErrorMock = vi
      .spyOn(console, "error")
      .mockImplementation(() => {});

    const { wrapper } = mountComponent({
      reservationCode: "TXN-noexiste"
    });

    global.fetch = vi.fn().mockResolvedValue({
      ok: false
    });

    await wrapper.vm.loadReservationDetails();

    expect(global.fetch).toHaveBeenCalledTimes(1);
    expect(wrapper.vm.errorMessage).toBe(
      "No se pudo cargar la información de la reserva."
    );

    consoleErrorMock.mockRestore();
  });

  it("formatea una fecha en formato costarricense", () => {
    const { wrapper } = mountComponent();
    const dateValue = "2026-07-01T10:00:00";

    const result = wrapper.vm.formatDate(dateValue);

    expect(result).toBe("01/07/2026");
  });

  it("devuelve texto vacío si no se recibe fecha al formatear", () => {
    const { wrapper } = mountComponent();

    const result = wrapper.vm.formatDate("");

    expect(result).toBe("");
  });

  it("formatea una fecha corta", () => {
    const { wrapper } = mountComponent();
    const dateValue = "2026-07-01T10:00:00";

    const result = wrapper.vm.formatShortDate(dateValue);

    expect(result.toLowerCase()).toContain("jul");
    expect(result).toContain("01");
  });

  it("devuelve texto vacío si no se recibe fecha corta", () => {
    const { wrapper } = mountComponent();

    const result = wrapper.vm.formatShortDate("");

    expect(result).toBe("");
  });

  it("calcula 0 días restantes si no recibe fecha de salida", () => {
    const { wrapper } = mountComponent();

    const result = wrapper.vm.calculateDaysLeft("");

    expect(result).toBe(0);
  });

  it("calcula 0 días restantes si la fecha del vuelo ya pasó", () => {
    const { wrapper } = mountComponent();
    const pastDate = "2020-01-01T10:00:00";

    const result = wrapper.vm.calculateDaysLeft(pastDate);

    expect(result).toBe(0);
  });
});