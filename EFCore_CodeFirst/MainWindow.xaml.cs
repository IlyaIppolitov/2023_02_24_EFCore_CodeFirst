using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EFCore_CodeFirst
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppDbContext _db = new AppDbContext();

        public MainWindow()
        {
            InitializeComponent();

        }

        // Финализатор - вызывается Garbage collector
        ~MainWindow()
        {
            _db.Dispose();
        }

        private async void buttonShowStudents_Click(object sender, RoutedEventArgs e)
        {
            await PutStudentsToDataGrid();
        }

        // 
        private async void studentsDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var caseStudent = _db.Students.GetType().GenericTypeArguments.ElementAt(0).Name;
            var caseVisitation = _db.Students.GetType().GenericTypeArguments.ElementAt(0).Name;

            // Проверка того что к DataGrid присвоен перечень студентов
            if (studentsDataGrid.ItemsSource.GetType().GenericTypeArguments.ElementAt(0).Name == caseStudent)
            {
                var student = new Student()
                {
                    Id = Guid.NewGuid(),
                    Name = "",
                    Surname = ""
                };
                await _db.Students.AddAsync(student);
                await _db.SaveChangesAsync();
                e.NewItem = student;
            }
            // В альтернативе пока нет необходимости
            else if (studentsDataGrid.ItemsSource.GetType().GenericTypeArguments.ElementAt(0).Name == caseVisitation)
            { }

            await UpdateComboBox();
        }

        // Отображение посещений в DataGrid
        private async void buttonShowVisitsByDate_Click(object sender, RoutedEventArgs e)
        {
            if (!calendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберете дату!");
                return;
            }

            try
            {
                // Отслеживаются EF
                var visitations = await _db.Visitations
                    .Where(v => v.Date.Equals(DateOnly.FromDateTime(calendar.SelectedDate.Value)))
                    .Join(_db.Students, v => v.StudentId, s => s.Id, (v, s) => new { v.Id, s.Name, s.Surname, v.Date }).ToListAsync();
                studentsDataGrid.ItemsSource = visitations;
            }
            catch (SqliteException ex) { MessageBox.Show("Отсутствует необходимая таблица для отображения\n" + ex.Message); }
            catch (ArgumentNullException ex) { MessageBox.Show("Отсутствует необходимая таблица для отображения\n" + ex.Message); }
        }

        // Заполнение таблиц случайными данными (для проверок)
        private async void buttonFillDataInTables_Click(object sender, RoutedEventArgs e)
        {
            // Заполнение данных
            for (int i = 0; i < 10; i++)
                await _db.Students.AddAsync(new Student() {Id = Guid.NewGuid(), Name = Faker.Name.First(), Surname = Faker.Name.Last(), Birthday = RandomDay() });
            
            await SaveChangesToDb();

            foreach (var student in _db.Students)
                await _db.Visitations.AddAsync(new Visitation() { Id = Guid.NewGuid(), StudentId = student.Id, Date = new DateOnly(2022, 02, 24) });

            foreach (var student in _db.Students)
                await _db.Visitations.AddAsync(new Visitation() { Id = Guid.NewGuid(), StudentId = student.Id, Date = DateOnly.FromDateTime(DateTime.Now) });

            await SaveChangesToDb();

            await UpdateComboBox();
        }

        // Кнопка удаления БД с подтверджением
        private async void buttonDeleteDb_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult deleteMessageBox = MessageBox.Show(
                "Вы уверены, что хотите удалить базу данных?", 
                "!Удаление БД!",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (deleteMessageBox != MessageBoxResult.Yes)
                return;

            try
            {
                await _db.Database.EnsureDeletedAsync();
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show($"Ошибка удаления базы данных: " + ex.Message.ToString());
            }
            MessageBox.Show($"База данных удалена!");
        }

        // Генерация случайных дат для заполнения таблиц
        private Random gen = new Random();
        DateOnly RandomDay()
        {
            DateTime start = new DateTime(1980, 1, 1);
            int range = (new DateTime(1990, 1, 1) - start).Days;
            return DateOnly.FromDateTime(start.AddDays(gen.Next(range)));
        }

        // Сохранение изменений в базе данных при изменении их в таблице DataGrid
        private async void studentsDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {

            await SaveChangesToDb();
        }

        // Запись данных о посещении в память
        private async void buttonPutVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!calendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберете дату!");
                return;
            }

            if (comboboxStudents.SelectedValue.ToString() is not { } studentId)
            {
                MessageBox.Show("Выберете студента!");
                return;
            }

            await _db.Visitations.AddAsync(new Visitation() { Id = Guid.NewGuid(), StudentId = new Guid(studentId), Date = DateOnly.FromDateTime(calendar.SelectedDate.Value) });
            await SaveChangesToDb();
        }

        // Задание параметров комбобокс по факту загрузки основного окна
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateComboBox();
        }

        private async void buttonDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            var caseStudent = _db.Students.GetType().GenericTypeArguments.ElementAt(0).Name;
            var caseVisitation = _db.Students.GetType().GenericTypeArguments.ElementAt(0).Name;

            if (studentsDataGrid.ItemsSource is null)
            {
                MessageBox.Show("Вы даже не открыли страницу со студентами, а уже хотите кого-то удалять! Как не стыдно!");
                return;
            }

            if (studentsDataGrid.ItemsSource.GetType().GenericTypeArguments.ElementAt(0).Name == caseStudent)
            {
                if (studentsDataGrid.SelectedItem is null)
                {
                    MessageBox.Show("Выберете студента для удаления!");
                    return;
                }

                var item = (Student) studentsDataGrid.SelectedItem;
                _db.Students.Remove(item);
                
                await SaveChangesToDb();
                
                MessageBox.Show("Студент удалён!");
            }

            await UpdateComboBox();
            await PutStudentsToDataGrid();
        }

        /// Сохранение изменений в базу данных
        private async Task SaveChangesToDb()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex) { MessageBox.Show("Ошибка! Данные были изменены с момента их загрузки в память!" + ex.Message); }
            catch (DbUpdateException ex) { MessageBox.Show("Ошибка сохранения в базу данных: " + ex.Message); }
        }

        /// Отправка перечня студентов в DataGrid
        private async Task PutStudentsToDataGrid()
        {
            try
            {
                // Отслеживаются EF
                var students = await _db.Students.ToListAsync();
                //var students = await _db.Students.Select(x => new { x.Id, x.Name, x.Surname, x.Birthday }).ToListAsync();
                studentsDataGrid.ItemsSource = students;
            }
            catch (SqliteException ex) { MessageBox.Show("Отсутствует необходимая таблица для отображения\n" + ex.Message); }
            catch (ArgumentNullException ex) { MessageBox.Show("Отсутствует необходимая таблица для отображения\n" + ex.Message); }
        }

        // Обновление перечня студентов в Combobox
        private async Task UpdateComboBox()
        {
            comboboxStudents.ItemsSource = await _db.Students.ToListAsync();
            comboboxStudents.SelectedValuePath = "Id";
            comboboxStudents.DisplayMemberPath = "Surname";
        }
    }
}
