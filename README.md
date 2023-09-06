# BTLabVue

1. **Установить MS SQL Express:**
Скачать и установить: (https://download.microsoft.com/download/5/1/4/5145fe04-4d30-4b85-b0d1-39533663a2f1/SQL2022-SSEI-Expr.exe)
Скачать и установить: (https://aka.ms/ssmsfullsetup)
Запустить SQL Server Management Studio (SMSS)
Создать базу данных с именем 'btlab'
В свойствах сервера на вкладке Безопасность выбрать "Проверка подлинности SQL Server и Windows"
Добавить пользователя с необходимыми правами для редактирования базы 'btlab'
Отредактировать строку подключения к БД в файле 'appsettings.json' в папке с сайтом

3. Установить Microsoft .NET 7.0.10 - Windows Server Hosting (https://download.visualstudio.microsoft.com/download/pr/d489c5d0-4d0f-4622-ab93-b0f2a3e92eed/101a2fae29a291956d402377b941f401/dotnet-hosting-7.0.10-win.exe)

4. Установить IIS (appwiz.cpl -> "Службы IIS") (https://wiki.merionet.ru/articles/ustanovka-iis-servera-na-windows-10)

5. Скачать и распаковать папку с публикацией сайта (https://drive.google.com/file/d/1_gF7EhsbkrSFqGwYuBqhO5di6SFzYkPD/view?usp=sharing)

6. Добавить сайт в IIS:
5.1 В консоли выполнить команду inetmgr
5.2 На папке 'сайты' нажать ПКМ -> 'Добавить веб-сайт...'
5.3 Указать произвольное 'Имя сайта'
5.4 Указать 'Физический путь' (путь до папки, в которой лежит сайт)
5.5 Указать тип https и имя узла 'localhost'
5.6 SSL-сертификат выбрать IIS Express Development Certificate

7. Проверить работы сайта (https://localhost)



Возможные ошибки:
1. Не работает SQL Server:
1.1 Запустить SQL Server Configuration Manager
1.2 На вкладке SQL Server запустить SQL Server (SQLEXPRESS)
