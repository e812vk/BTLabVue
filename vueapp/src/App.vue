<script setup>
    import TableElement from './components/TableElement.vue'
</script>

<template>
    <header></header>
    <main>
        <div class="d-grid gap-3">
            <div style="padding-top: 35px;">
                <h1 class="text-center">Табель учета отсутствия сотрудников</h1>
            </div>
            <div class="wrapper">
                <TableElement />
            </div>
        </div>
    </main>
</template>

<style scoped>
    header {
        line-height: 1.5;
    }

    .logo {
        display: block;
        margin: 0 auto 2rem;
    }

    @media (min-width: 1024px) {
        header {
            display: flex;
            place-items: center;
            padding-right: calc(var(--section-gap) / 2);
        }

        .logo {
            margin: 0 2rem 0 0;
        }

        header .wrapper {
            display: flex;
            place-items: flex-start;
            flex-wrap: wrap;
        }
    }
</style>
