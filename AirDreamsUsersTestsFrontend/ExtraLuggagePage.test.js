import { describe, it, expect, beforeEach, vi } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ExtraLuggagePage from '../Sprint_1/AirDreams/frontend/src/views/ExtraLuggagePage.vue'

const createPassenger = ({
  idPassenger = 1,
  fullName = 'Adrian',
  initialChecked = 0,
  initialCarryOn = 0,
  initialTotal = 0,
  currentChecked = 0,
  currentCarryOn = 0,
  currentTotal = 0,
  extraChecked = 0,
  extraCarryOn = 0,
  extraTotal = 0
} = {}) => ({
  idPassenger,
  fullName,
  luggage: {
    initial: {
      checkedCount: initialChecked,
      carryOnCount: initialCarryOn,
      totalPrice: initialTotal
    },
    current: {
      checkedCount: currentChecked,
      carryOnCount: currentCarryOn,
      totalPrice: currentTotal
    },
    extra: {
      checkedCount: extraChecked,
      carryOnCount: extraCarryOn,
      totalPrice: extraTotal
    }
  }
})

const mountComponent = (reservationCode = 'TXN-123') => {
  return shallowMount(ExtraLuggagePage, {
    global: {
      mocks: {
        $route: {
          query: { reservationCode }
        },
        $router: {
          push: vi.fn()
        }
      },
      stubs: {
        ExtraLuggagePassenger: true,
        PopupMessage: true
      }
    }
  })
}

describe('ExtraLuggagePage.vue - unit tests de lógica', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    vi.spyOn(ExtraLuggagePage.methods, 'loadPageData').mockResolvedValue()
  })

  it('obtiene el reservationCode desde la query y elimina espacios', () => {
    const wrapper = mountComponent('  TXN-123  ')

    expect(wrapper.vm.reservationCode).toBe('TXN-123')
  })

  it('devuelve string vacío si no viene reservationCode', () => {
    const wrapper = shallowMount(ExtraLuggagePage, {
      global: {
        mocks: {
          $route: { query: {} },
          $router: { push: vi.fn() }
        },
        stubs: {
          ExtraLuggagePassenger: true,
          PopupMessage: true
        }
      }
    })

    expect(wrapper.vm.reservationCode).toBe('')
  })

  it('calcula los totales de equipaje correctamente', () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        initialTotal: 15000,
        currentTotal: 38000,
        extraTotal: 23000
      }),
      createPassenger({
        idPassenger: 2,
        initialTotal: 8000,
        currentTotal: 8000,
        extraTotal: 0
      })
    ]

    expect(wrapper.vm.previousLuggageTotal).toBe(23000)
    expect(wrapper.vm.newLuggageTotal).toBe(46000)
    expect(wrapper.vm.extraLuggageTotal).toBe(23000)
  })

  it('devuelve 0 en los totales si no hay pasajeros', () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = []

    expect(wrapper.vm.previousLuggageTotal).toBe(0)
    expect(wrapper.vm.newLuggageTotal).toBe(0)
    expect(wrapper.vm.extraLuggageTotal).toBe(0)
  })

  it('calcula el peso total de maletas extra', () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        extraChecked: 2,
        extraCarryOn: 3
      }),
      createPassenger({
        idPassenger: 2,
        extraChecked: 1,
        extraCarryOn: 1
      })
    ]

    expect(wrapper.vm.totalCheckedWeight).toBe(69)
    expect(wrapper.vm.totalCarryOnWeight).toBe(40)
  })

  it('handleLuggageChange actualiza current y extra correctamente', async () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        initialChecked: 1,
        initialCarryOn: 0,
        initialTotal: 15000,
        currentChecked: 1,
        currentCarryOn: 0,
        currentTotal: 15000
      })
    ]

    vi.spyOn(wrapper.vm, 'calculateLuggageTotal').mockResolvedValue(38000)

    await wrapper.vm.handleLuggageChange({
      passengerIndex: 1,
      checkedCount: 2,
      carryOnCount: 1
    })

    expect(wrapper.vm.pageState.passengers[0].luggage.current).toEqual({
      checkedCount: 2,
      carryOnCount: 1,
      totalPrice: 38000
    })

    expect(wrapper.vm.pageState.passengers[0].luggage.extra).toEqual({
      checkedCount: 1,
      carryOnCount: 1,
      totalPrice: 23000
    })
  })

  it('handleLuggageChange no genera valores negativos en extra', async () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        initialChecked: 2,
        initialCarryOn: 1,
        initialTotal: 38000,
        currentChecked: 2,
        currentCarryOn: 1,
        currentTotal: 38000
      })
    ]

    vi.spyOn(wrapper.vm, 'calculateLuggageTotal').mockResolvedValue(15000)

    await wrapper.vm.handleLuggageChange({
      passengerIndex: 1,
      checkedCount: 1,
      carryOnCount: 0
    })

    expect(wrapper.vm.pageState.passengers[0].luggage.extra).toEqual({
      checkedCount: 0,
      carryOnCount: 0,
      totalPrice: 0
    })
  })

  it('handleLuggageChange muestra error si no encuentra el pasajero', async () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = []

    const showErrorSpy = vi.spyOn(wrapper.vm, 'showError')

    await wrapper.vm.handleLuggageChange({
      passengerIndex: 999,
      checkedCount: 1,
      carryOnCount: 0
    })

    expect(showErrorSpy).toHaveBeenCalledWith('No se encontró el pasajero.')
  })

  it('buildExtraLuggageByPassenger devuelve solo pasajeros con equipaje extra', () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        fullName: 'Adrian',
        initialTotal: 15000,
        currentTotal: 38000,
        extraChecked: 1,
        extraCarryOn: 1,
        extraTotal: 23000
      }),
      createPassenger({
        idPassenger: 2,
        fullName: 'Maria',
        initialTotal: 8000,
        currentTotal: 8000
      })
    ]

    expect(wrapper.vm.buildExtraLuggageByPassenger()).toEqual([
      expect.objectContaining({
        idPassenger: 1,
        luggageItems: [
          { type: 'checked', quantity: 1 },
          { type: 'carryOn', quantity: 1 }
        ],
        previousTotal: 15000,
        newTotal: 38000,
        passengerTotal: 23000,
        passenger: expect.objectContaining({
          idPassenger: 1,
          fullName: 'Adrian'
        })
      })
    ])
  })

  it('buildExtraLuggageByPassenger devuelve vacío si nadie agregó equipaje', () => {
    const wrapper = mountComponent()

    wrapper.vm.pageState.passengers = [
      createPassenger({
        idPassenger: 1,
        initialTotal: 15000,
        currentTotal: 15000
      })
    ]

    expect(wrapper.vm.buildExtraLuggageByPassenger()).toEqual([])
  })
})