<template>
  <div>
    <HeaderLogo />
    <NavigationReports
      :current-view="currentView"
      @change-view="currentView = $event"
    />
    <main class="reports-page">
      <section v-if="currentView === 'sales'" class="report-section">
        <div class="section-heading">
          <h1>Reporte de ingresos</h1>
          <p>
            Consulta los ingresos por tiquetes y maletas agrupados por mes.
          </p>
        </div>

        <div class="report-grid">
          <article class="summary-card">
            <span class="summary-label">Ingresos totales</span>
            <strong>{{ formatCurrency(incomeSummary.totalIngresos) }}</strong>
          </article>

          <article class="summary-card">
            <span class="summary-label">Ingresos por tiquetes</span>
            <strong>{{ formatCurrency(incomeSummary.ingresosTiquetes) }}</strong>
          </article>

          <article class="summary-card">
            <span class="summary-label">Ingresos por maletas</span>
            <strong>{{ formatCurrency(incomeSummary.ingresosMaletas) }}</strong>
          </article>
        </div>

        <div class="report-panel">
          <div class="panel-heading">
            <h2>Detalle mensual</h2>
            <div class="panel-actions">
              <button
                class="secondary-btn"
                type="button"
                :disabled="incomeReport.length === 0 || preparingExcel"
                @click="exportIncomeReportToExcel"
              >
                {{ preparingExcel ? 'Preparando...' : 'Exportar XLSX' }}
              </button>

              <button class="refresh-btn" type="button" @click="getIncomeReport">
                Actualizar
              </button>
            </div>
          </div>

          <form class="filters-bar" @submit.prevent="getIncomeReport">
            <label>
              Origen
              <select v-model="incomeFilters.origin">
                <option value="">Todos</option>
                <option
                  v-for="origin in incomeFilterOptions.origins"
                  :key="origin"
                  :value="origin"
                >
                  {{ origin }}
                </option>
              </select>
            </label>

            <label>
              Destino
              <select v-model="incomeFilters.destination">
                <option value="">Todos</option>
                <option
                  v-for="destination in incomeFilterOptions.destinations"
                  :key="destination"
                  :value="destination"
                >
                  {{ destination }}
                </option>
              </select>
            </label>

            <label>
              Año
              <select v-model="incomeFilters.year">
                <option value="">Todos</option>
                <option
                  v-for="year in incomeFilterOptions.years"
                  :key="year"
                  :value="year"
                >
                  {{ year }}
                </option>
              </select>
            </label>

            <button class="filter-btn" type="submit">
              Filtrar
            </button>
          </form>

          <p v-if="loadingIncomeReport" class="state-message">
            Cargando reporte de ingresos...
          </p>

          <p v-else-if="incomeReportError" class="state-message error">
            {{ incomeReportError }}
          </p>

          <p v-else-if="incomeReport.length === 0" class="state-message">
            No hay datos de ingresos para mostrar.
          </p>

          <table v-else class="report-table">
            <thead>
              <tr>
                <th>Mes</th>
                <th>Vuelos</th>
                <th>Primera clase</th>
                <th>Clase económica</th>
                <th>Total pasajeros</th>
                <th>Maletas</th>
                <th>Ingresos tiquetes</th>
                <th>Ingresos maletas</th>
                <th>Total</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="row in incomeReport" :key="`${row.anio}-${row.mesNumero}`">
                <td>{{ row.mes }}</td>
                <td>{{ row.cantidadVuelos }}</td>
                <td>{{ row.totalPasajerosPrimeraClase }}</td>
                <td>{{ row.totalPasajerosClaseEconomica }}</td>
                <td>{{ row.totalPasajeros }}</td>
                <td>
                  {{ row.totalMaletas }}
                  <span class="muted-detail">
                    ({{ row.totalMaletasDocumentadas }} doc. / {{ row.totalMaletasCarryOn }} carry on)
                  </span>
                </td>
                <td>{{ formatCurrency(row.ingresosTiquetes) }}</td>
                <td>{{ formatCurrency(row.ingresosMaletas) }}</td>
                <td class="total-cell">{{ formatCurrency(row.totalIngresos) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <section v-if="currentView === 'flights'" class="report-section">
        <div class="section-heading">
          <h1>Reporte de vuelos</h1>
          <p>Consulta detallada de vuelos, pasajeros e ingresos.</p>
        </div>

        <div class="report-grid">
          <article class="summary-card">
            <span class="summary-label">Total vuelos</span>
            <strong>{{ flightsReport.length }}</strong>
          </article>
          <article class="summary-card">
            <span class="summary-label">Total pas. 1ª clase</span>
            <strong>{{ flightsTotals.firstClassTotal }}</strong>
          </article>
          <article class="summary-card">
            <span class="summary-label">Total pas. turista</span>
            <strong>{{ flightsTotals.touristTotal }}</strong>
          </article>
          <article class="summary-card">
            <span class="summary-label">Ingresos totales</span>
            <strong>{{ formatCurrency(flightsTotals.grandTotal) }}</strong>
          </article>
        </div>

        <div class="report-panel">
          <div class="panel-heading">
            <h2>Detalle por vuelo</h2>
            <div class="panel-actions">
              <button
                class="secondary-btn"
                :disabled="flightsReport.length === 0 || preparingFlightsExcel"
                @click="exportFlightsToExcel"
              >
                {{ preparingFlightsExcel ? 'Preparando...' : 'Exportar XLSX' }}
              </button>
              <button class="refresh-btn" @click="getFlightsReport">Actualizar</button>
            </div>
          </div>

          <form class="filters-bar" @submit.prevent="getFlightsReport">
            <label>
              Origen
              <select v-model="flightsFilters.origin">
                <option value="">Todos</option>
                <option
                  v-for="o in flightsFilterOptions.origins"
                  :key="o"
                  :value="o"
                >{{ o }}</option>
              </select>
            </label>
            <label>
              Destino
              <select v-model="flightsFilters.destination">
                <option value="">Todos</option>
                <option
                  v-for="d in flightsFilterOptions.destinations"
                  :key="d"
                  :value="d"
                >{{ d }}</option>
              </select>
            </label>
            <label>
              Clase
              <select v-model="flightsFilters.seatClass">
                <option value="">Todas</option>
                <option value="FirstClass">Primera clase</option>
                <option value="Turista">Turista</option>
              </select>
            </label>
            <label>
              Desde
              <input type="date" v-model="flightsFilters.fromDate" />
            </label>
            <label>
              Hasta
              <input type="date" v-model="flightsFilters.toDate" />
            </label>
            <button class="filter-btn" type="submit">Filtrar</button>
          </form>

          <p v-if="loadingFlights" class="state-message">Cargando reporte de vuelos...</p>
          <p v-else-if="flightsError" class="state-message error">{{ flightsError }}</p>
          <p v-else-if="flightsReport.length === 0" class="state-message">No hay vuelos que mostrar.</p>

          <table v-else class="report-table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Origen</th>
                <th>Destino</th>
                <th>Número de vuelo</th>
                <th>Pasajeros 1ª clase</th>
                <th>Pasajeros turista</th>
                <th>Aerolínea</th>
                <th>Venta pasajeros</th>
                <th>Venta equipajes</th>
                <th>Total venta</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in flightsReport" :key="row.numberFlight">
                <td>{{ formatDate(row.flightDate) }}</td>
                <td>{{ row.origin }}</td>
                <td>{{ row.destination }}</td>
                <td>{{ row.numberFlight }}</td>
                <td>{{ row.firstClassPassengers ?? '-' }}</td>
                <td>{{ row.touristPassengers ?? '-' }}</td>
                <td>{{ row.airline }}</td>
                <td>{{ formatCurrency(row.passengerRevenue) }}</td>
                <td>{{ formatCurrency(row.luggageRevenue) }}</td>
                <td class="total-cell">{{ formatCurrency(row.totalRevenue) }}</td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="total-row">
                <td colspan="4">TOTALES</td>
                <td>{{ flightsTotals.firstClassTotal }}</td>
                <td>{{ flightsTotals.touristTotal }}</td>
                <td></td>
                <td>{{ formatCurrency(flightsTotals.passengerRevenueTotal) }}</td>
                <td>{{ formatCurrency(flightsTotals.luggageRevenueTotal) }}</td>
                <td class="total-cell">{{ formatCurrency(flightsTotals.grandTotal) }}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      </section>
    </main>
  </div>
</template>

<script>
import HeaderLogo from '../components/HeaderLogo.vue'
import NavigationReports from '../components/NavigationReports.vue'
import axios from 'axios'
import ExcelJS from 'exceljs'

export default {
  name: 'ReportsPage',
  components: { HeaderLogo, NavigationReports },
  data() {
    return {
      currentView: 'sales',
      incomeReport: [],
      incomeFilterOptions: { origins: [], destinations: [], years: [] },
      incomeFilters: { origin: '', destination: '', year: '' },
      loadingIncomeReport: false,
      incomeReportError: '',
      preparingExcel: false,
      excelDownloadUrl: '',
      excelDownloadName: '',

      flightsReport: [],
      flightsFilterOptions: { origins: [], destinations: [], minDate: '', maxDate: '' },
      flightsFilters: { origin: '', destination: '', seatClass: '', fromDate: '', toDate: '' },
      loadingFlights: false,
      flightsError: '',
      preparingFlightsExcel: false
    }
  },
  computed: {
    incomeSummary() {
      return this.incomeReport.reduce((summary, row) => {
        summary.totalIngresos += Number(row.totalIngresos) || 0
        summary.ingresosTiquetes += Number(row.ingresosTiquetes) || 0
        summary.ingresosMaletas += Number(row.ingresosMaletas) || 0
        return summary
      }, { totalIngresos: 0, ingresosTiquetes: 0, ingresosMaletas: 0 })
    },

    flightsTotals() {
      return this.flightsReport.reduce((totals, row) => {
        totals.firstClassTotal += (row.firstClassPassengers || 0)
        totals.touristTotal += (row.touristPassengers || 0)
        totals.passengerRevenueTotal += (row.passengerRevenue || 0)
        totals.luggageRevenueTotal += (row.luggageRevenue || 0)
        totals.grandTotal += (row.totalRevenue || 0)
        return totals
      }, {
        firstClassTotal: 0,
        touristTotal: 0,
        passengerRevenueTotal: 0,
        luggageRevenueTotal: 0,
        grandTotal: 0
      })
    }
  },
  created() {
    this.getIncomeReportFilters()
    this.getIncomeReport()
    this.getFlightsReportFilters()
    this.getFlightsReport()
  },
  beforeUnmount() {
    this.revokeIncomeReportExcelUrl()
  },
  methods: {
    async getIncomeReport() {
      this.loadingIncomeReport = true
      this.incomeReportError = ''
      try {
        const params = {}
        if (this.incomeFilters.origin) params.origin = this.incomeFilters.origin
        if (this.incomeFilters.destination) params.destination = this.incomeFilters.destination
        if (this.incomeFilters.year) params.year = this.incomeFilters.year
        const response = await axios.get('http://localhost:5276/api/reports/income', { params })
        this.incomeReport = response.data
      } catch (error) {
        this.incomeReportError = error.response?.data?.error || 'No se pudo cargar el reporte de ingresos.'
      } finally {
        this.loadingIncomeReport = false
      }
    },

    async getIncomeReportFilters() {
      try {
        const response = await axios.get('http://localhost:5276/api/reports/income/filters')
        this.incomeFilterOptions = {
          origins: response.data.origins || [],
          destinations: response.data.destinations || [],
          years: response.data.years || []
        }
      } catch (error) {
        console.error('No se pudieron cargar los filtros del reporte:', error)
      }
    },

    buildIncomeReportRows() {
      return this.incomeReport.map(row => ({
        mes: row.mes,
        vuelos: row.cantidadVuelos,
        primeraClase: row.totalPasajerosPrimeraClase,
        claseEconomica: row.totalPasajerosClaseEconomica,
        totalPasajeros: row.totalPasajeros,
        maletasDocumentadas: row.totalMaletasDocumentadas,
        maletasCarryOn: row.totalMaletasCarryOn,
        totalMaletas: row.totalMaletas,
        ingresosTiquetes: Number(row.ingresosTiquetes) || 0,
        ingresosMaletas: Number(row.ingresosMaletas) || 0,
        totalIngresos: Number(row.totalIngresos) || 0
      }))
    },

    getIncomeReportFileName(extension) {
      const origin = this.incomeFilters.origin || 'todos-origenes'
      const destination = this.incomeFilters.destination || 'todos-destinos'
      const year = this.incomeFilters.year || 'todos-años'
      return `reporte-ingresos-${origin}-${destination}-${year}.${extension}`
        .toLowerCase()
        .replace(/\s+/g, '-')
    },

    getIncomeReportFilterLabel() {
      return [
        `Origen: ${this.incomeFilters.origin || 'Todos'}`,
        `Destino: ${this.incomeFilters.destination || 'Todos'}`,
        `Año: ${this.incomeFilters.year || 'Todos'}`
      ].join(' | ')
    },

    revokeIncomeReportExcelUrl() {
      if (!this.excelDownloadUrl) return
      URL.revokeObjectURL(this.excelDownloadUrl)
      this.excelDownloadUrl = ''
      this.excelDownloadName = ''
    },

    async exportIncomeReportToExcel() {
      const reportRows = this.buildIncomeReportRows()
      if (!reportRows.length) {
        this.incomeReportError = 'No hay datos disponibles para exportar.'
        return
      }
      this.preparingExcel = true
      this.incomeReportError = ''
      this.revokeIncomeReportExcelUrl()
      try {
        const workbook = new ExcelJS.Workbook()
        workbook.creator = 'AirDreams'
        workbook.created = new Date()
        const worksheet = workbook.addWorksheet('Reporte de ingresos', {
          views: [{ state: 'frozen', ySplit: 4 }],
          pageSetup: { orientation: 'landscape', fitToPage: true, fitToWidth: 1, fitToHeight: 0 }
        })
        worksheet.properties.showGridLines = false
        worksheet.columns = [
          { width: 16 }, { width: 16 }, { width: 20 }, { width: 22 }, { width: 19 },
          { width: 21 }, { width: 18 }, { width: 18 }, { width: 22 }, { width: 21 }, { width: 20 }
        ]
        const border = {
          top: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          left: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          bottom: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          right: { style: 'thin', color: { argb: 'FFD9E2F3' } }
        }
        worksheet.mergeCells('A1:K1')
        const titleCell = worksheet.getCell('A1')
        titleCell.value = 'Reporte de ingresos - AirDreams'
        titleCell.font = { name: 'Calibri', size: 16, bold: true, color: { argb: 'FFFFFFFF' } }
        titleCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1F4E78' } }
        titleCell.alignment = { horizontal: 'center', vertical: 'middle' }
        worksheet.getRow(1).height = 28
        worksheet.mergeCells('A2:K2')
        const generatedCell = worksheet.getCell('A2')
        generatedCell.value = `${this.getIncomeReportFilterLabel()} | Generado el ${new Date().toLocaleString('es-CR')}`
        generatedCell.font = { name: 'Calibri', size: 10, italic: true, color: { argb: 'FF595959' } }
        generatedCell.alignment = { horizontal: 'center', vertical: 'middle' }
        worksheet.getRow(2).height = 18
        worksheet.getRow(3).height = 8
        const headerRow = worksheet.getRow(4)
        headerRow.values = [
          'Mes', 'Cantidad de vuelos', 'Pasajeros primera clase', 'Pasajeros clase economica',
          'Total de pasajeros', 'Maletas documentadas', 'Maletas carry-on', 'Total de maletas',
          'Ingresos por tiquetes', 'Ingresos por maletas', 'Total de ingresos'
        ]
        headerRow.height = 34
        headerRow.eachCell(cell => {
          cell.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'FFFFFFFF' } }
          cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF305496' } }
          cell.alignment = { horizontal: 'center', vertical: 'middle', wrapText: true }
          cell.border = border
        })
        reportRows.forEach((row, index) => {
          const excelRow = worksheet.addRow([
            row.mes, Number(row.vuelos ?? 0), Number(row.primeraClase ?? 0),
            Number(row.claseEconomica ?? 0), Number(row.totalPasajeros ?? 0),
            Number(row.maletasDocumentadas ?? 0), Number(row.maletasCarryOn ?? 0),
            Number(row.totalMaletas ?? 0), Number(row.ingresosTiquetes ?? 0),
            Number(row.ingresosMaletas ?? 0), Number(row.totalIngresos ?? 0)
          ])
          excelRow.height = 20
          excelRow.eachCell((cell, columnNumber) => {
            cell.border = border
            cell.alignment = { horizontal: columnNumber === 1 ? 'left' : 'right', vertical: 'middle' }
            if (index % 2 === 1) {
              cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FFF3F7FC' } }
            }
          })
          for (let column = 2; column <= 8; column++) excelRow.getCell(column).numFmt = '#,##0'
          for (let column = 9; column <= 11; column++) excelRow.getCell(column).numFmt = '"₡"#,##0.00'
        })
        const firstDataRow = 5
        const lastDataRow = firstDataRow + reportRows.length - 1
        const totalRow = worksheet.addRow([
          'TOTAL',
          { formula: `SUM(B${firstDataRow}:B${lastDataRow})` },
          { formula: `SUM(C${firstDataRow}:C${lastDataRow})` },
          { formula: `SUM(D${firstDataRow}:D${lastDataRow})` },
          { formula: `SUM(E${firstDataRow}:E${lastDataRow})` },
          { formula: `SUM(F${firstDataRow}:F${lastDataRow})` },
          { formula: `SUM(G${firstDataRow}:G${lastDataRow})` },
          { formula: `SUM(H${firstDataRow}:H${lastDataRow})` },
          { formula: `SUM(I${firstDataRow}:I${lastDataRow})` },
          { formula: `SUM(J${firstDataRow}:J${lastDataRow})` },
          { formula: `SUM(K${firstDataRow}:K${lastDataRow})` }
        ])
        totalRow.height = 24
        totalRow.eachCell((cell, columnNumber) => {
          cell.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'FFFFFFFF' } }
          cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1F4E78' } }
          cell.border = border
          cell.alignment = { horizontal: columnNumber === 1 ? 'left' : 'right', vertical: 'middle' }
        })
        for (let column = 2; column <= 8; column++) totalRow.getCell(column).numFmt = '#,##0'
        for (let column = 9; column <= 11; column++) totalRow.getCell(column).numFmt = '"₡"#,##0.00'
        worksheet.autoFilter = { from: 'A4', to: `K${lastDataRow}` }
        worksheet.getColumn(1).alignment = { horizontal: 'left', vertical: 'middle' }
        const buffer = await workbook.xlsx.writeBuffer()
        const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
        this.excelDownloadName = this.getIncomeReportFileName('xlsx')
        this.excelDownloadUrl = URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = this.excelDownloadUrl
        link.download = this.excelDownloadName
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
      } catch (error) {
        console.error('Error al generar XLSX:', error)
        this.incomeReportError = 'No se pudo generar el archivo XLSX.'
      } finally {
        this.preparingExcel = false
      }
    },

    async getFlightsReport() {
      this.loadingFlights = true
      this.flightsError = ''
      try {
        const params = { ...this.flightsFilters }
        if (!params.fromDate) delete params.fromDate
        if (!params.toDate) delete params.toDate
        const response = await axios.get('http://localhost:5276/api/reports/flights', { params })
        this.flightsReport = response.data
      } catch (error) {
        this.flightsError = error.response?.data?.error || 'Error al cargar el reporte de vuelos.'
      } finally {
        this.loadingFlights = false
      }
    },

    async getFlightsReportFilters() {
      try {
        const res = await axios.get('http://localhost:5276/api/reports/flights/filters')
        this.flightsFilterOptions.origins = res.data.origins || []
        this.flightsFilterOptions.destinations = res.data.destinations || []
        if (res.data.minDate) this.flightsFilterOptions.minDate = res.data.minDate
        if (res.data.maxDate) this.flightsFilterOptions.maxDate = res.data.maxDate
      } catch (e) {
        console.error('Filtros de vuelos no disponibles', e)
      }
    },

    async exportFlightsToExcel() {
      if (!this.flightsReport.length) return
      this.preparingFlightsExcel = true
      this.flightsError = ''
      try {
        const workbook = new ExcelJS.Workbook()
        workbook.creator = 'AirDreams'
        workbook.created = new Date()
        const sheet = workbook.addWorksheet('Reporte de vuelos', {
          views: [{ state: 'frozen', ySplit: 1 }],
          pageSetup: { orientation: 'landscape', fitToPage: true, fitToWidth: 1, fitToHeight: 0 }
        })
        sheet.properties.showGridLines = false
        sheet.columns = [
          { header: 'Fecha', key: 'flightDate', width: 14 },
          { header: 'Origen', key: 'origin', width: 10 },
          { header: 'Destino', key: 'destination', width: 10 },
          { header: 'Número de vuelo', key: 'numberFlight', width: 18 },
          { header: 'Pasajeros 1ª clase', key: 'firstClass', width: 18 },
          { header: 'Pasajeros turista', key: 'tourist', width: 18 },
          { header: 'Aerolínea', key: 'airline', width: 14 },
          { header: 'Venta pasajeros', key: 'passengerRevenue', width: 18 },
          { header: 'Venta equipajes', key: 'luggageRevenue', width: 18 },
          { header: 'Total venta', key: 'totalRevenue', width: 18 }
        ]
        const border = {
          top: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          left: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          bottom: { style: 'thin', color: { argb: 'FFD9E2F3' } },
          right: { style: 'thin', color: { argb: 'FFD9E2F3' } }
        }
        sheet.mergeCells('A1:J1')
        const titleCell = sheet.getCell('A1')
        titleCell.value = 'Reporte de vuelos - AirDreams'
        titleCell.font = { name: 'Calibri', size: 16, bold: true, color: { argb: 'FFFFFFFF' } }
        titleCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1F4E78' } }
        titleCell.alignment = { horizontal: 'center', vertical: 'middle' }
        sheet.getRow(1).height = 28
        sheet.mergeCells('A2:J2')
        const filterLabel = `Origen: ${this.flightsFilters.origin || 'Todos'} | Destino: ${this.flightsFilters.destination || 'Todos'} | Clase: ${this.flightsFilters.seatClass || 'Todas'} | Desde: ${this.flightsFilters.fromDate || '-'} | Hasta: ${this.flightsFilters.toDate || '-'} | Generado: ${new Date().toLocaleString('es-CR')}`
        const filterCell = sheet.getCell('A2')
        filterCell.value = filterLabel
        filterCell.font = { name: 'Calibri', size: 10, italic: true, color: { argb: 'FF595959' } }
        filterCell.alignment = { horizontal: 'center', vertical: 'middle' }
        sheet.getRow(2).height = 18
        sheet.getRow(3).height = 8
        const headerRow = sheet.getRow(4)
        headerRow.values = [
          'Fecha', 'Origen', 'Destino', 'Número de vuelo',
          'Pasajeros 1ª clase', 'Pasajeros turista', 'Aerolínea',
          'Venta pasajeros', 'Venta equipajes', 'Total venta'
        ]
        headerRow.height = 34
        headerRow.eachCell(cell => {
          cell.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'FFFFFFFF' } }
          cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF305496' } }
          cell.alignment = { horizontal: 'center', vertical: 'middle', wrapText: true }
          cell.border = border
        })
        this.flightsReport.forEach((row, index) => {
          const excelRow = sheet.addRow([
            this.formatDate(row.flightDate),
            row.origin,
            row.destination,
            row.numberFlight,
            row.firstClassPassengers ?? '',
            row.touristPassengers ?? '',
            row.airline,
            Number(row.passengerRevenue ?? 0),
            Number(row.luggageRevenue ?? 0),
            Number(row.totalRevenue ?? 0)
          ])
          excelRow.height = 20
          excelRow.eachCell((cell, columnNumber) => {
            cell.border = border
            cell.alignment = { horizontal: columnNumber >= 8 ? 'right' : 'left', vertical: 'middle' }
            if (index % 2 === 1) {
              cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FFF3F7FC' } }
            }
          })
          excelRow.getCell(8).numFmt = '"₡"#,##0.00'
          excelRow.getCell(9).numFmt = '"₡"#,##0.00'
          excelRow.getCell(10).numFmt = '"₡"#,##0.00'
        })
        const firstDataRow = 5
        const lastDataRow = firstDataRow + this.flightsReport.length - 1
        const totalRow = sheet.addRow([
          'TOTALES',
          '', '', '',
          this.flightsTotals.firstClassTotal,
          this.flightsTotals.touristTotal,
          '',
          this.flightsTotals.passengerRevenueTotal,
          this.flightsTotals.luggageRevenueTotal,
          this.flightsTotals.grandTotal
        ])
        totalRow.height = 24
        totalRow.eachCell((cell, columnNumber) => {
          cell.font = { name: 'Calibri', size: 11, bold: true, color: { argb: 'FFFFFFFF' } }
          cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF1F4E78' } }
          cell.border = border
          cell.alignment = { horizontal: columnNumber >= 8 ? 'right' : 'left', vertical: 'middle' }
        })
        totalRow.getCell(8).numFmt = '"₡"#,##0.00'
        totalRow.getCell(9).numFmt = '"₡"#,##0.00'
        totalRow.getCell(10).numFmt = '"₡"#,##0.00'
        sheet.autoFilter = { from: 'A4', to: `J${lastDataRow}` }
        const buffer = await workbook.xlsx.writeBuffer()
        const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
        const link = document.createElement('a')
        link.href = URL.createObjectURL(blob)
        link.download = `reporte-vuelos-${new Date().toISOString().slice(0,10)}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
      } catch (error) {
        console.error('Error al generar XLSX de vuelos:', error)
        this.flightsError = 'No se pudo generar el archivo Excel.'
      } finally {
        this.preparingFlightsExcel = false
      }
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        maximumFractionDigits: 2
      }).format(Number(value) || 0)
    },

    formatDate(dateStr) {
      if (!dateStr) return ''
      const date = new Date(dateStr)
      return date.toLocaleDateString('es-CR', { day: '2-digit', month: '2-digit', year: 'numeric' })
    }
  }
}
</script>

<style scoped>
.reports-page {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px 40px 48px;
  font-family: 'Inter', sans-serif;
}

.report-section {
  width: 100%;
}

.section-heading {
  margin-bottom: 28px;
}

.section-heading h1 {
  color: #384467;
  font-size: 40px;
  font-weight: 700;
  margin: 0 0 8px;
}

.section-heading p {
  color: #6b7280;
  font-size: 16px;
  margin: 0;
}

.report-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
  margin-bottom: 28px;
}

.summary-card,
.placeholder-panel,
.report-panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.summary-card {
  padding: 20px;
  min-height: 116px;
}

.summary-label {
  display: block;
  color: #6b7280;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 18px;
}

.summary-card strong {
  color: #384467;
  font-size: 24px;
  font-weight: 700;
}

.placeholder-panel {
  padding: 24px;
  color: #384467;
}

.placeholder-panel h2 {
  font-size: 20px;
  font-weight: 700;
  margin: 0 0 8px;
}

.placeholder-panel p {
  color: #6b7280;
  margin: 0;
}

.report-panel {
  padding: 24px;
  overflow-x: auto;
}

.panel-heading {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
}

.panel-heading h2 {
  color: #384467;
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}

.panel-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: flex-end;
}

.refresh-btn {
  border: none;
  border-radius: 30px;
  background-color: #3d4b74;
  color: white;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 600;
  padding: 8px 18px;
}

.secondary-btn {
  border: 1px solid #3d4b74;
  border-radius: 30px;
  background-color: white;
  color: #3d4b74;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 600;
  padding: 8px 18px;
}

.secondary-btn:disabled {
  border-color: #cbd5e1;
  color: #94a3b8;
  cursor: not-allowed;
}

.state-message {
  color: #6b7280;
  margin: 0;
}

.state-message.error {
  color: #b42318;
}

.filters-bar {
  display: grid;
  grid-template-columns: repeat(3, minmax(160px, 1fr)) auto;
  gap: 14px;
  margin-bottom: 20px;
}

.filters-bar label {
  color: #384467;
  display: flex;
  flex-direction: column;
  font-size: 14px;
  font-weight: 600;
  gap: 6px;
}

.filters-bar select,
.filters-bar input {
  border: 1px solid #d7dce8;
  border-radius: 8px;
  color: #384467;
  font-family: 'Inter', sans-serif;
  min-height: 38px;
  padding: 7px 10px;
}

.filter-btn {
  align-self: end;
  border: none;
  border-radius: 8px;
  background-color: #f5dddd;
  color: #3d4b74;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 700;
  min-height: 38px;
  padding: 8px 22px;
}

.report-table {
  width: 100%;
  border-collapse: collapse;
  color: #384467;
  min-width: 1100px;
}

.report-table th,
.report-table td {
  border-bottom: 1px solid #f1f5f9;
  padding: 12px 14px;
  text-align: left;
}

.report-table th {
  background-color: #f8fafc;
  color: #334155;
  font-weight: 700;
}

.total-cell {
  font-weight: 700;
}

.total-row {
  background-color: #f8fafc;
  font-weight: 700;
}

.muted-detail {
  color: #6b7280;
  font-size: 13px;
}

@media (max-width: 900px) {
  .report-grid {
    grid-template-columns: 1fr;
  }

  .filters-bar {
    grid-template-columns: 1fr;
  }

  .filter-btn {
    width: 100%;
  }

  .panel-heading {
    align-items: flex-start;
    flex-direction: column;
  }

  .panel-actions {
    justify-content: flex-start;
    width: 100%;
  }
}

@media (max-width: 768px) {
  .reports-page {
    padding: 20px;
  }

  .section-heading h1 {
    font-size: 32px;
  }
}
</style>