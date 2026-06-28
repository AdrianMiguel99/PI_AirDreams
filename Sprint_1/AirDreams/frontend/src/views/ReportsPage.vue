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

            <label>
              Aerolínea
              <select v-model="incomeFilters.airline">
                <option value="">Todas</option>
                <option
                  v-for="airline in incomeFilterOptions.airlines"
                  :key="airline"
                  :value="airline"
                >
                  {{ airline }}
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
          <p>
            Vista inicial para revisar rutas, ocupacion y datos operativos de vuelos.
          </p>
        </div>

        <div class="report-grid">
          <article class="summary-card">
            <span class="summary-label">Vuelos registrados</span>
            <strong>Pendiente</strong>
          </article>

          <article class="summary-card">
            <span class="summary-label">Rutas activas</span>
            <strong>Pendiente</strong>
          </article>

          <article class="summary-card">
            <span class="summary-label">Ocupacion promedio</span>
            <strong>Pendiente</strong>
          </article>
        </div>

        <div class="placeholder-panel">
          <h2>Detalle del reporte</h2>
          <p>
            Este espacio queda listo para mostrar informacion del segundo tipo de reporte.
          </p>
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

  components: {
    HeaderLogo,
    NavigationReports
  },

  data() {
    return {
      currentView: 'sales',
      incomeReport: [],
      incomeFilterOptions: {
        origins: [],
        destinations: [],
        years: [],
        airlines: []
      },
      incomeFilters: {
        origin: '',
        destination: '',
        year: '',
        airline: ''
      },
      loadingIncomeReport: false,
      incomeReportError: '',
      preparingExcel: false,
      excelDownloadUrl: '',
      excelDownloadName: ''
    }
  },

  computed: {
    incomeSummary() {
      return this.incomeReport.reduce((summary, row) => {
        summary.totalIngresos += Number(row.totalIngresos) || 0
        summary.ingresosTiquetes += Number(row.ingresosTiquetes) || 0
        summary.ingresosMaletas += Number(row.ingresosMaletas) || 0
        return summary
      }, {
        totalIngresos: 0,
        ingresosTiquetes: 0,
        ingresosMaletas: 0
      })
    }
  },

  created() {
    this.getIncomeReportFilters()
    this.getIncomeReport()
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
        if (this.incomeFilters.airline) params.airline = this.incomeFilters.airline

        const response = await axios.get('http://localhost:5276/api/reports/income', {
          params
        })
        this.incomeReport = response.data
      } catch (error) {
        this.incomeReportError =
          error.response?.data?.error ||
          'No se pudo cargar el reporte de ingresos.'
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
          years: response.data.years || [],
          airlines: response.data.airlines || []
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
      const airline = this.incomeFilters.airline || 'todas-aerolineas'

      return `reporte-ingresos-${origin}-${destination}-${year}-${airline}.${extension}`
        .toLowerCase()
        .replace(/\s+/g, '-')
    },

    getIncomeReportFilterLabel() {
      return [
        `Origen: ${this.incomeFilters.origin || 'Todos'}`,
        `Destino: ${this.incomeFilters.destination || 'Todos'}`,
        `Año: ${this.incomeFilters.year || 'Todos'}`,
        `Aerolínea: ${this.incomeFilters.airline || 'Todas'}`
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
          pageSetup: {
            orientation: 'landscape',
            fitToPage: true,
            fitToWidth: 1,
            fitToHeight: 0
          }
        })

        worksheet.properties.showGridLines = false
        worksheet.columns = [
          { width: 16 },
          { width: 16 },
          { width: 20 },
          { width: 22 },
          { width: 19 },
          { width: 21 },
          { width: 18 },
          { width: 18 },
          { width: 22 },
          { width: 21 },
          { width: 20 }
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
        titleCell.font = {
          name: 'Calibri',
          size: 16,
          bold: true,
          color: { argb: 'FFFFFFFF' }
        }
        titleCell.fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FF1F4E78' }
        }
        titleCell.alignment = {
          horizontal: 'center',
          vertical: 'middle'
        }
        worksheet.getRow(1).height = 28

        worksheet.mergeCells('A2:K2')
        const generatedCell = worksheet.getCell('A2')
        generatedCell.value = `${this.getIncomeReportFilterLabel()} | Generado el ${new Date().toLocaleString('es-CR')}`
        generatedCell.font = {
          name: 'Calibri',
          size: 10,
          italic: true,
          color: { argb: 'FF595959' }
        }
        generatedCell.alignment = {
          horizontal: 'center',
          vertical: 'middle'
        }
        worksheet.getRow(2).height = 18
        worksheet.getRow(3).height = 8

        const headerRow = worksheet.getRow(4)
        headerRow.values = [
          'Mes',
          'Cantidad de vuelos',
          'Pasajeros primera clase',
          'Pasajeros clase economica',
          'Total de pasajeros',
          'Maletas documentadas',
          'Maletas carry-on',
          'Total de maletas',
          'Ingresos por tiquetes',
          'Ingresos por maletas',
          'Total de ingresos'
        ]
        headerRow.height = 34
        headerRow.eachCell(cell => {
          cell.font = {
            name: 'Calibri',
            size: 11,
            bold: true,
            color: { argb: 'FFFFFFFF' }
          }
          cell.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FF305496' }
          }
          cell.alignment = {
            horizontal: 'center',
            vertical: 'middle',
            wrapText: true
          }
          cell.border = border
        })

        reportRows.forEach((row, index) => {
          const excelRow = worksheet.addRow([
            row.mes,
            Number(row.vuelos ?? 0),
            Number(row.primeraClase ?? 0),
            Number(row.claseEconomica ?? 0),
            Number(row.totalPasajeros ?? 0),
            Number(row.maletasDocumentadas ?? 0),
            Number(row.maletasCarryOn ?? 0),
            Number(row.totalMaletas ?? 0),
            Number(row.ingresosTiquetes ?? 0),
            Number(row.ingresosMaletas ?? 0),
            Number(row.totalIngresos ?? 0)
          ])

          excelRow.height = 20
          excelRow.eachCell((cell, columnNumber) => {
            cell.border = border
            cell.alignment = {
              horizontal: columnNumber === 1 ? 'left' : 'right',
              vertical: 'middle'
            }

            if (index % 2 === 1) {
              cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'FFF3F7FC' }
              }
            }
          })

          for (let column = 2; column <= 8; column++) {
            excelRow.getCell(column).numFmt = '#,##0'
          }

          for (let column = 9; column <= 11; column++) {
            excelRow.getCell(column).numFmt = '"₡"#,##0.00'
          }
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
          cell.font = {
            name: 'Calibri',
            size: 11,
            bold: true,
            color: { argb: 'FFFFFFFF' }
          }
          cell.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FF1F4E78' }
          }
          cell.border = border
          cell.alignment = {
            horizontal: columnNumber === 1 ? 'left' : 'right',
            vertical: 'middle'
          }
        })

        for (let column = 2; column <= 8; column++) {
          totalRow.getCell(column).numFmt = '#,##0'
        }

        for (let column = 9; column <= 11; column++) {
          totalRow.getCell(column).numFmt = '"₡"#,##0.00'
        }

        worksheet.autoFilter = {
          from: 'A4',
          to: `K${lastDataRow}`
        }

        worksheet.getColumn(1).alignment = {
          horizontal: 'left',
          vertical: 'middle'
        }

        const buffer = await workbook.xlsx.writeBuffer()
        const blob = new Blob([buffer], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })

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

    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        maximumFractionDigits: 2
      }).format(Number(value) || 0)
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

.filters-bar select {
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

.download-link {
  color: #3d4b74;
  display: inline-block;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 16px;
  text-decoration: underline;
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
