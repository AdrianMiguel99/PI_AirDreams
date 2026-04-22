<template>
    <!-- Icono de aerolinea y texto de la aerolinea-->
    <div class="d-flex align-items-center">
    <img 
        src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png" class="img-fluid" alt="imagen de aerolinea" style="width:50px"
    >
    <h2 class="letra_semibold" style="font-size: 1.3rem; padding-top: 10px;">Air Dreams</h2>

    </div>

    <!--Se crea un contenedor para el titulo principal del formulario de registro de aeronave-->
    <div class="container mt-5">

        <!-- Titulo principal -->
        <h1 class="letra_bold text-center" style="font-size: 2.5rem;">
            Registro de Aeronave
        </h1>

        <!-- Boton de listar aeronaves-->
         <div class="row justify-content-end letra_bold" >
            <div class="col-2">
                <a href="">
                    <button type="button" class="btn btn-outline-secondary boton_listar">
                        Listar Aeronaves
                    </button>
                </a>
            </div>
        </div>
    </div>

    <!--Formulario de registro de aeronave-->
    <div class="d-flex justify-content-center align-items-center vh-75 mt-4">
        <div class="letra_semibold p-3 shadow" style="max-width: 400px; width: 40%;">
            <form>
                <div class="mb-3">
                    <!-- TODO: QUE NO SE REPITA EN EL SISTEMA-->
                    <label for="matricula" class="form-label">Matrícula (10 caracteres max)</label>
                    <input type="text" maxlength="10" class="form-control ." id="matricula" placeholder="Ejm: TI-BFJ">
                </div>
                <div class="mb-3">
                    <label for="peso_general" class="form-label">Peso (kg)</label>
                    <input type="text" class="form-control" id="peso_general" placeholder="Ejm: 1000">
                </div>
                <div class="mb-3">
                    <label for="peso_equipaje" class="form-label">Peso Equipaje (kg)</label>
                    <input type="number" class="form-control" id="peso_equipaje" placeholder="Ejm: 500">
                </div>
                <div class="mb-3">
                    <label for="peso_equipaje" class="form-label">Peso Equipaje (kg)</label>
                    <input type="number" class="form-control" id="peso_equipaje" placeholder="Ejm: 500">
                </div>
                <button type="submit" class="btn colores_invertidos w-100" style="background-color: #384467; font-weight: semi-bold">Registrar Aeronave</button>
            </form>
        </div>
    </div>




</template>

<script>
    export default {}
</script>

<style scoped>
    .letra_bold {
        font-family:  'Inter', sans-serif;
        color: #384467;
        font-weight: bold;
    }

    .letra_semibold {
        font-family: 'Inter', sans-serif;
        font-weight: semi-bold;
        color: #384467;
    }

    .colores_invertidos {
        font-family: 'Inter', sans-serif;
        background-color: #384467;
        color: white;
        font-weight: semi-bold;
    }

    .boton_listar {
        font-family: 'Inter', sans-serif;
        color: #384467;
        font-weight: semi-bold;
        border: 2px solid #384467;
    }

    .boton_listar:hover {
        background-color: #384467;
        color: white;
    }
</style>