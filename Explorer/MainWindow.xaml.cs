using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Explorer
{
    public partial class MainWindow : Window
    {
        private string path = "C:/";//путь
        private bool is_file = false;//файл/нет
        private string item_name="";//имя текущего файла
        private string old_path = "C:/";//старый путь
        private List<string> paths = new List<string>();//массив путей для переходов
        private int current_index = 0;//текущий индекс
        public MainWindow()
        {
            InitializeComponent();
            paths.Add(path); //добавим путь в историю
            LoadAll(); //выгружаем файлы и папки в ListView
        }

        //выгружаем файлы и папки в ListView и если это файл он открывается
        private void LoadAll()
        {
            DirectoryInfo file_list;
            try{
                if (is_file){//если это файл запускаем его
                    Process.Start(path);//запуск файла
                    path = old_path;
                    return;
                }
                //если это папка то открываем ее (загружаем и папки и файлы)
                file_list = new DirectoryInfo(path);
                ListViewFiles.Items.Clear();
                DirectoryInfo[] dirs = file_list.GetDirectories().Where(c=>!c.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();//папки (не скрытые)
                for (int i = 0; i < dirs.Length; i++)
                    ListViewFiles.Items.Add(dirs[i].Name);
                FileInfo[] files = file_list.GetFiles().Where(c => !c.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();//файлы (не скрытые)
                for (int i = 0; i < files.Length; i++)
                    ListViewFiles.Items.Add(files[i].Name);
                //выводим путь в стоку поиска
                this_path.Text = path;
            }
            catch (Exception e){
                path = old_path;
            }
        }

        //при выборе (1 клике) выводим информацию об объекте
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //если объект выбран - получаем его название
            try {item_name = e.AddedItems[0].ToString();}
            catch {item_name=null;}
            //проверяем папка это или файл
            FileAttributes fileatr = File.GetAttributes(path + "/" + item_name);//получаем атрибуты
            if ((fileatr & FileAttributes.Directory) == FileAttributes.Directory) is_file = false;//папка
            else is_file = true;//файл
            FileInfo file_info = new FileInfo(path + "/" + item_name);
            //заполняем информацию в лейблы
            if ((String.IsNullOrEmpty(file_info.Name))){//если ничего не выбрано выводим сообщение
                FileName.Content = "Select file or folder";
                TypeFile.Content = null;
                ObjectPath.Content = null;
                CreateDate.Content = null;
                EditeDate.Content = null;
                SizeOrCount.Content = null;
            }
            else
            {//иначе заполняем информацию в лейблы
                FileName.Content = "Item name: " + file_info.Name;
                TypeFile.Content = "Item type: " + (String.IsNullOrEmpty(file_info.Extension) ? "folder" : file_info.Extension);
                ObjectPath.Content = "Item path: " + path + "/" + item_name;
                CreateDate.Content = "Item creation date: " + File.GetCreationTime(path + "/" + item_name);
                EditeDate.Content = "Item modification date: " + File.GetLastWriteTime(path + "/" + item_name);
                if(String.IsNullOrEmpty(file_info.Extension)){
                    try{
                        int count = Directory.GetDirectories(path + "/" + item_name, "*", SearchOption.TopDirectoryOnly).Length;
                        count += Directory.GetFiles(path + "/" + item_name, "*", SearchOption.TopDirectoryOnly).Length;
                        SizeOrCount.Content = "Count files: " + count;
                    }
                    catch { SizeOrCount.Content = "Count files: 0"; }
                }
                else{
                    //если папка начинается с "."
                    //
                    try
                    {
                        long size = file_info.Length;
                        if (size > 1024 * 1024 * 1024) { size = size / (1024 * 1024 * 1024); SizeOrCount.Content = "File size: " + size + " Gb"; }
                        if (size > 1024 * 1024) { size = size / (1024 * 1024); SizeOrCount.Content = "File size: " + size + " Mb"; }
                        if ((size > 1024)) { size = size / 1024; SizeOrCount.Content = "File size: " + size + " Kb"; }
                        else SizeOrCount.Content = "File size: " + size + " Byte";
                    }
                    catch { }
                }
            }
        }

        //добавим путь в историю
        private void path_counter()
        {
            if (current_index < paths.Count - 1) {
                List<string> temp = new List<string>();//
                for (int i = 0; i <= current_index; i++)
                    temp.Add(paths[i]);
                paths = temp;
            }
            paths.Add(path);
            current_index++;
            this_path.Items.Clear();
            for (int i = 0; i < paths.Count; i++)
                if(!this_path.Items.Contains(paths[i])) this_path.Items.Add(paths[i]);
        }

        //даблклик - переход по пути/открытие файла
        private void ListViewFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            old_path = path;
            path = path + (String.IsNullOrEmpty(item_name)?item_name:"/" + item_name);
            path_counter();
            LoadAll();
        }

        //переход вперед (из истории)
        private void ForwardBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            is_file = false;
            if (current_index == paths.Count-1) return;
            old_path = path;
            path = paths[current_index + 1];//смотрим на следующий элемент в пути
            current_index++;
            LoadAll();
        }

        //переход назад (из истории)
        private void BackBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            is_file = false;
            if (current_index == 0) return;
            old_path = path;
            path = paths[current_index - 1];//смотрим на предыдущий элемент в пути
            current_index--;
            LoadAll();
        }

        //переход выше на уровень (по пути)
        private void Topbtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            is_file = false;
            old_path = path;//запоминаем путь
            //если можем вернуться назад - убираем последний элемент из пути
            if (path != "C:/") path = path.Substring(0, path.LastIndexOf("/"));
            else return;
            path_counter();
            LoadAll();//обновляем
        }

        //при нажатии Enter обновлять путь
        private void this_path_KeyDown(object sender, KeyEventArgs e)
        {
            is_file = false;
            if (e.Key != Key.Enter) return;//если нажат не Enter - ничего не происходит
            old_path = path;
            path = this_path.Text;//добавим введенный путь
            path_counter();
            LoadAll();
        }

        //поиск фала
        private void FindBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Search();
        }

        //при нажатии Enter поиск файла
        private void this_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Search();//если нажат Enter - поиск
        }

        //поиск файла
        private void Search()
        {
            if (String.IsNullOrEmpty(this_search.Text)) { LoadAll(); return; }
            ListViewFiles.Items.Clear();//чистим
            var file_list = new DirectoryInfo(path);
            //заполняем
            try{
                DirectoryInfo[] dirs = file_list.GetDirectories(this_search.Text, SearchOption.TopDirectoryOnly).Where(c => !c.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();//папки (не скрытые)
                for (int i = 0; i < dirs.Length; i++)
                    ListViewFiles.Items.Add(dirs[i].Name);
            }
            catch { }
            try{
                FileInfo[] files = file_list.GetFiles(this_search.Text, SearchOption.TopDirectoryOnly).Where(c => !c.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();//файлы (не скрытые)
                for (int i = 0; i < files.Length; i++)
                    ListViewFiles.Items.Add(files[i].Name);
            }
            catch { }
        }

        //переход по выбранному пути
        private void this_path_DropDownClosed(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this_path.Text)) return;
            is_file = false;
            old_path = path;
            path = this_path.Text;//добавим введенный путь
            path_counter();
            LoadAll();
        }
    }
}
