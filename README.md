# data-serialization
Преобразование xml файла в файл json.

Консольное приложение (.net 5). Протестировано на mac и windows.

На вход подается целевой xml файл (test.xml), который должен находится в папке с исполняемым файлом. 
Выходной файл (output.json) будет расположен в текущей папке с исполняемым файлом.

Описание

Так как файл xml файл может иметь очень большой размер использование библиотек типа newton json.net и прочих техник с полной загрузкой текста файла в память
будут иметь негативные последствия. 
Для наибольшей эффективности по памяти был выбран способ inline преобразования xml в выходной json файл по полной программе используя потоки с минимальной буферизацией. 
Этот способ в некоторой степени ограничивает возможности типизации, расширяемости и абстракции от конкретных тегов, однако, позволяет получить минимальные показатели по расходу памяти. В случае необходимости можно и этот проект абстрагировать от некотрых конкретных реализаций

Как работает

После запуска консоли будет выведено информационное сообщение. Нажмите любую кнопку для запуска процесса конвертации xml файла в json. После преобразования 
файла будет выведено информационное сообщение об успехе операции или в случае ошибки - текст с ошибкой и стектрейс. 
Выходной файл будет расположен в папке с испольняемым файлом.