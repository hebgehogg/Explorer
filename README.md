# Тестовое задание в компанию Аэросфера
- Овчинникова Алина 
- hebgehogg@gmail.com
- тел: +7(950) 446-12-02


> Постановка задачи:

Реализовать файловый менеджер, отображающий файловую систему, на языке C#. 

Графический интерфейс приложения должен быть написан на WPF и быть интуитивно понятным.
Файловый менеджер обладает следующими особенностями:
* Состоит из одной области("рабочая область"), в которой отображается содержимое текущей директории (или папки)
* При двойном нажатии на элемент списка содержимого:
   а) если это файл, то приложение пытается его открыть в Windows;
   б) если это папка, то в рабочую область загружается уже содержимое данной папки

* При одинарном нажатии на элемент списка содержимого, в правой стороне рабочей
области должна появляться панель, в которой отображается дополнительная информация:
   а) если это файл, то его метаданные(размер, дата создания, ...);
   б) если это папка, то размер и количество файлов в ней

Усложнения (Усложнения можно делать или не делать, на свой выбор)

1) Реализовать в верхней части рабочей зоны путь в иерархии папок. При нажатии на папку
в иерархии пути, происходит переход в данную папку
2) Реализовать функцию поиска по имени. Добавлется область (или окно на свое усмотрение), 
в которой при вводе текста появляется список релеватных файлов в данной папке. 
При нажатии на файл приложение пытается его открыть в Windows.

> Результат:

![Скриншот интерфейса](https://github.com/hebgehogg/Explorer/blob/master/FileExplorer.png)
