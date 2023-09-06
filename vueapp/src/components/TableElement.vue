<template>
    <div>
        <div v-if="loading" class="loading">
            Загрузка данных...
        </div>
        <div v-if="connectionError">
            Проверьте соединение с сервером
        </div>
        <div v-if="databaseError" class="mx-auto form-control text-md-center">
            <div>
                <h3>Не удалось соединиться с сервером базы данных.</h3>
            </div>
            <div>
                <button type="button" class="btn btn-outline-primary btn-lg" @click="createTable">
                    Попробовать создать таблицу в базе данных
                </button>
            </div>
        </div>
        <div v-if="table" class="content">
            <div class="mx-auto form-control text-md-center" style="max-width: 1500px;">
                <table class="table table-striped">
                    <thead>
                        <tr class="align-middle">
                            <th>Идентификатор</th>
                            <th>Причина отсутствия</th>
                            <th>Дата начала</th>
                            <th>Продолжительность<br />(раб.дней)</th>
                            <th>Учтено при оплате</th>
                            <th>Комментарий</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="table.length < 1">
                            <td colspan="7">Нет записей в таблице</td>
                        </tr>
                        <tr v-else class="align-middle" v-for="record in table" :key="record.id">
                            <td>{{ record.id }}</td>
                            <td>{{ reasons[record.reason].desc }}</td>
                            <td>{{ record.start_date }}</td>
                            <td>{{ record.duration }}</td>
                            <td>{{ record.discounted  ? 'да' : 'нет'}}</td>
                            <td>{{ record.description }}</td>
                            <td>
                                <div class="d-grid gap-2 d-md-flex justify-content-md-end">
                                    <button type="button" class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#recordModal" @click="editRecord(record)">
                                        Изменить
                                    </button>
                                    <button type="button" class="btn btn-outline-secondary" @click="deleteRecord(record.id)">
                                        Удалить
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <br />
            <div class="text-md-center">
                <button type="button" class="btn btn-outline-primary btn-lg" data-bs-toggle="modal" data-bs-target="#recordModal">
                    Создать запись
                </button>
            </div>
        </div>
    </div>

    <!-- Модальное окно создания/редактирования записи -->
    <div class="modal fade" id="recordModal" tabindex="-1" aria-labelledby="recordModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <div v-if="recordModalMode">
                        <h1 class="modal-title fs-5" id="recordModalLabel">Создать запись</h1>
                    </div>
                    <div v-else>
                        <h1 class="modal-title fs-5" id="recordModalLabel">Редактировать запись</h1>
                    </div>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="form-input">
                        <label for="reason" class="form-label">Причина отсутствия</label>
                        <select class="form-select" id="reason" v-model="record.reason" placeholder="test">
                            <option v-for="reason in reasons" :value="reason.code">{{ reason.desc }}</option>
                        </select>
                    </div>
                    <div class="form-input">
                        <label class="form-label" for="start_date">Дата начала</label>
                        <input class="form-control" type="date" id="start_date" v-model="record.start_date" />
                    </div>
                    <div class="form-input">
                        <label for="duration" class="form-label">Продолжительность (раб.дней)</label>
                        <input class="form-control" type="number" id="duration" v-model="record.duration" />
                    </div>
                    <div class="form-check">
                        <label class="form-check-label" for="discounted">Учтено при оплате</label>
                        <input class="form-check-input" type="checkbox" id="discounted" v-model="record.discounted" />
                    </div>
                    <div class="form-input">
                        <label for="description" class="form-label">Комментарий</label>
                        <textarea class="form-control" id="description" v-model="record.description" rows="3" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div v-if="recordModalMode">
                        <button type="button" class="btn btn-outline-primary" @click="addRecord(record)">
                            Создать
                        </button>
                    </div>
                    <div v-else>
                        <button type="button" class="btn btn-outline-primary" @click="saveRecord">
                            Сохранить
                        </button>
                    </div>
                    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal" @click="resetRecordFields">Отмена</button>
                </div>
                <div v-if="validMessage" class="alert alert-danger">
                    {{ validMessage }}
                </div>
            </div>
        </div>
    </div>

    <!-- Модальное окно для отображения сообщений -->
    <div class="modal fade" id="messageModal" tabindex="-1" aria-labelledby="messageModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="messageModalLabel">Сообщение</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    {{ message }}
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">ОK</button>
                </div>
            </div>
        </div>
    </div>

</template>

<script>
    import { defineComponent } from 'vue';
    
    export default defineComponent({
        data() {
            return {
                loading: false,
                connectionError: false,
                databaseError: false,
                table: null,
                validMessage: null,
                message: null,
                record: {
                    id: 0,
                    reason: null,
                    start_date: null,
                    duration: null,
                    discounted: false,
                    description: null
                },
                reasons: [
                    { code: 0, desc: 'Прогул' },
                    { code: 1, desc: 'Больничный' },
                    { code: 2, desc: 'Отпуск' }
                ],
                recordModalMode: true
            };
        },

        created() {
            this.fetchRecords();
        },

        methods: {
            // Запрос всех записей с сервера
            async fetchRecords() {
                this.table = null;
                this.loading = true;
                this.connectionError = false;
                this.databaseError = false;
                // Запрос на сервер с таймаутом ожидания ответа.
                try {
                    var data = await this.fetchWithTimeout('getRecords', { timeout: 10000 })
                        .then(response => response.json());

                    // Обработка если ответ от сервера получен.
                    if (data.isSuccess) {
                        // Отображение результатов при нормальной работе.
                        this.table = data.args;
                        this.loading = false;
                    }
                    else {
                        // Отображение результатов при ошибке обращения сервера к БД.
                        this.loading = false;
                        this.databaseError = true;
                        this.showMessage(data.message);
                    }
                }
                // Отображение при ошибке обращения к серверу.
                catch
                {
                    this.table = null;
                    this.loading = false;
                    this.connectionError = true;
                }
            },

            // Попытка создания таблицы, например, в случае первого запуска.
            async createTable() {
                var data = await this.fetchWithTimeout('createTable', { timeout: 15000 })
                    .then(response => response.json());

                if (!data.isSuccess) this.showMessage(data.message);
                else {
                    await this.refreshAllData();
                    this.showMessage(data.message);
                }
            },

            // Запуск запроса к серверу с таймаутом.
            async fetchWithTimeout(resource, options = {}) {
                const controller = new AbortController();
                const id = setTimeout(() => controller.abort(), options.timeout);
                const response = await fetch(resource, {
                    options,
                    signal: controller.signal
                });
                clearTimeout(id);
                return response;
            },

            // Добавление записи на сервере.
            async addRecord() {
                if (!this.validateForm()) return;

                const requestOptions = this.getRequestOptions();
                const data = await fetch('addRecord', requestOptions)
                    .then((response) => response.json());
                this.closeModal();
                this.showMessage(data.message);
            },

            // Заполнение формы редактирования записи.
            async editRecord(record) {
                this.recordModalMode = false;
                this.record.id = record.id;
                this.record.reason = record.reason;
                this.record.start_date = record.start_date;
                this.record.duration = record.duration;
                this.record.discounted = record.discounted;
                this.record.description = record.description;
            },

            // Сохранение измененной записи.
            async saveRecord() {
                if (!this.validateForm()) return;

                const requestOptions = this.getRequestOptions();
                const data = await fetch('editRecord', requestOptions)
                    .then((response) => response.json());
                this.closeModal();
                this.showMessage(data.message);
            },

            // Удаление записи по идентификатору.
            async deleteRecord(id) {
                const data = await fetch(`deleteRecord?id=${id}`)
                    .then((response) => response.json());
                this.showMessage(`Запись ${id} удалена`);
                await this.refreshAllData();
            },

            // Обновление данных таблицы
            async refreshAllData() {
                this.recordModalMode = true;
                this.resetRecordFields();
                await this.fetchRecords();
            },

            // Отображение сообщения в модальном окне
            async showMessage(message) {
                const messageModal = new bootstrap.Modal(document.getElementById('messageModal'));
                await messageModal.toggle(this.message = message);
            },

            // Закрытие модального окна редактирования/создания записи
            async closeModal() {
                var modalElement = document.getElementById('recordModal');
                var modal = bootstrap.Modal.getInstance(modalElement);
                await modal.hide();
                await this.refreshAllData();
            },

            // Формирований опций для отправки POST запроса
            getRequestOptions(record) {
                return {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.record)
                };
            },

            // Сброс полей добавления/редактирования записи
            resetRecordFields() {
                this.record.reason = null;
                this.record.id = 0;
                this.record.start_date = null;
                this.record.duration = null;
                this.record.discounted = false;
                this.record.description = null;
                this.validMessage = null;
            },

            // Проверка заполнения полей формы добавления/редактирования записи
            validateForm() {
                this.validMessage = this.getErrorMessage();
                return !this.validMessage;
            },

            // Выбор сообщения для отображения при проверке формы
            getErrorMessage() {
                if (this.record.reason === null) return 'Укажите причину отсутствия';
                if (!this.record.start_date) return 'Укажите дату начала отсутствия';
                if (this.record.duration < 1) return 'Укажите продолжительность отсутствия';
                if (!this.record.description || this.record.description === '') return 'Напишите комментарий';
            }
        },
    });
</script>
