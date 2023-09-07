# BTLabVue
Инструкция по разворачиванию сайта на Windows 10 (64-bit).

1. **Установить и настроить MS SQL Express:**
   - Скачать и установить: (https://download.microsoft.com/download/5/1/4/5145fe04-4d30-4b85-b0d1-39533663a2f1/SQL2022-SSEI-Expr.exe)
   - Скачать и установить: (https://aka.ms/ssmsfullsetup)
   - Запустить SQL Server Management Studio (SMSS)
   - Создать базу данных с именем 'btlab'
   - В свойствах сервера на вкладке Безопасность выбрать "Проверка подлинности SQL Server и Windows"
   - Перезапустить SQL-сервер
   - Добавить пользователя с необходимыми правами для редактирования базы 'btlab'
   - Отредактировать строку подключения к БД в файле 'appsettings.json' в папке с сайтом

3. **Установить Microsoft .NET 7.0.10 - Windows Server Hosting** (https://download.visualstudio.microsoft.com/download/pr/d489c5d0-4d0f-4622-ab93-b0f2a3e92eed/101a2fae29a291956d402377b941f401/dotnet-hosting-7.0.10-win.exe)

4. **Установить IIS Express** чтобы появился самозаверенный сертификат (https://www.microsoft.com/en-US/download/details.aspx?id=48264)

5. **Установить IIS** (appwiz.cpl -> Включение или отключение компонентов Windows -> "Службы IIS") (https://wiki.merionet.ru/articles/ustanovka-iis-servera-na-windows-10)

6. **Дать доступ на папку с сайтом группе пользователей IIS_IUSRS**

7. **Скачать и распаковать папку с публикацией сайта** (https://drive.google.com/file/d/1_gF7EhsbkrSFqGwYuBqhO5di6SFzYkPD/view?usp=sharing)

8. **Добавить сайт в IIS:**
   - В консоли выполнить команду inetmgr
   - На папке 'сайты' нажать ПКМ -> 'Добавить веб-сайт...'
   - Указать произвольное 'Имя сайта'
   - Пул приложений 'DefaultAppPull'
   - Указать 'Физический путь' (путь до папки, в которой лежит сайт)
   - Указать тип https и имя узла 'localhost'
   - SSL-сертификат выбрать IIS Express Development Certificate

9. **Проверить работу сайта** (https://localhost)



Возможные ошибки:
1. **Не работает SQL Server:**
   - Запустить SQL Server Configuration Manager
   - На вкладке SQL Server запустить SQL Server (SQLEXPRESS)

2. Основные ошибки IIS связанные с размещением сайта описываются Microsoft (https://learn.microsoft.com/ru-ru/troubleshoot/developer/webapps/iis/health-diagnostic-performance/http-error-500-19-webpage)
   
3. Решение ошибки SSL-сертификата для localhost с использованием PowerShell (команды выделены курсивом):
   - создаем новый корневой доверенный сертификат:
     _$rootCert = New-SelfSignedCertificate -Subject 'CN=TestRootCA,O=TestRootCA,OU=TestRootCA' -KeyExportPolicy Exportable -KeyUsage CertSign,CRLSign,DigitalSignature -KeyLength 2048 -KeyUsageProperty All -KeyAlgorithm 'RSA' -HashAlgorithm 'SHA256'  -Provider 'Microsoft Enhanced RSA and AES Cryptographic Provider'_
   - создаем сертификат из корневой цепочки доверенных сертификатов:
     _New-SelfSignedCertificate -DnsName "localhost" -FriendlyName "MyCert" -CertStoreLocation "cert:\LocalMachine\My" -Signer $rootCert -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") -Provider "Microsoft Strong Cryptographic Provider" -HashAlgorithm "SHA256" -NotAfter (Get-Date).AddYears(10)_

   - копируем **отпечаток**, возвращенный последней командой

   - удаляем последнюю ассоциацию ip/port/cert:
     _netsh http delete sslcert ipport=0.0.0.0:443_

   - привязываем новый сертификат к любому ip и порту 443 (значение appid не имеет значения, подходит любой валидный guid):
     _netsh http add sslcert ipport=0.0.0.0:443 appid='{214124cd-d05b-4309-9af9-9caa44b2b74a}' certhash=**сюда_вставить_отпечаток**_

   - Теперь вам необходимо открыть MMC (в термиинале набрать 'certlm.msc') и перетащить сертификат TestRootCA из подпапки «Личные/Сертификаты» в подпапку «Доверенные корневые центры сертификации/Сертификаты».
  

