using lab13.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace lab13.ViewModel
{
    public class BookViewModel : ObservableObject
    {
        private string _author;
        private string _title;
        private int _circulation;
        private decimal _price;
        private int _year;
        private string _searchKeyword = "Убийство";
        private string _resultText;
        private List<Book> _books = new List<Book>();
        private readonly string _filePath = "books.dat";
        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int Circulation
        {
            get => _circulation;
            set => SetProperty(ref _circulation, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        // ==========================================
        // КОМАНДЫ (привязка к кнопкам в XAML)
        // ==========================================
        public ICommand AddBookCommand { get; }
        public ICommand SaveToBinaryCommand { get; }
        public ICommand LoadFromBinaryCommand { get; }
        public ICommand SearchAuthorsCommand { get; }
        public ICommand AddSampleDataCommand { get; }

        // ==========================================
        // КОНСТРУКТОР
        // ==========================================
        public BookViewModel()
        {
            AddBookCommand = new RelayCommand(_ => AddBook());
            SaveToBinaryCommand = new RelayCommand(_ => SaveToBinary(), _ => _books.Count > 0);
            LoadFromBinaryCommand = new RelayCommand(_ => LoadFromBinary());
            SearchAuthorsCommand = new RelayCommand(_ => SearchAuthors(), _ => _books.Count > 0);
            AddSampleDataCommand = new RelayCommand(_ => AddSampleData());
        }

        // ==========================================
        // МЕТОДЫ
        // ==========================================

        private void AddBook()
        {
            if (string.IsNullOrWhiteSpace(Author) || string.IsNullOrWhiteSpace(Title))
            {
                ResultText = "Ошибка: заполните автора и название книги!";
                return;
            }

            var book = new Book
            {
                Author = Author,
                Title = Title,
                Circulation = Circulation,
                Price = Price,
                Year = Year
            };

            _books.Add(book);
            ResultText = $"Книга добавлена: {Title} ({Author}). Всего книг: {_books.Count}";

            Author = "";
            Title = "";
            Circulation = 0;
            Price = 0;
            Year = 0;
        }

        private void SaveToBinary()
        {
            try
            {
                using (var fs = new FileStream(_filePath, FileMode.Create))
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(_books.Count);
                    foreach (var book in _books)
                    {
                        writer.Write(book.Author!);
                        writer.Write(book.Title!);
                        writer.Write(book.Circulation);
                        writer.Write(book.Price);
                        writer.Write(book.Year);
                    }
                }
                ResultText = $"Данные сохранены в бинарный файл: {_filePath}. Записей: {_books.Count}";
            }
            catch (Exception ex)
            {
                ResultText = $"Ошибка сохранения: {ex.Message}";
            }
        }

        private void LoadFromBinary()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    ResultText = "Файл не найден!";
                    return;
                }

                _books.Clear();
                using (var fs = new FileStream(_filePath, FileMode.Open))
                using (var reader = new BinaryReader(fs))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        _books.Add(new Book
                        {
                            Author = reader.ReadString(),
                            Title = reader.ReadString(),
                            Circulation = reader.ReadInt32(),
                            Price = reader.ReadDecimal(),
                            Year = reader.ReadInt32()
                        });
                    }
                }
                ResultText = $"Загружено {_books.Count} книг из файла {_filePath}.";
            }
            catch (Exception ex)
            {
                ResultText = $"Ошибка загрузки: {ex.Message}";
            }
        }

        private void SearchAuthors()
        {
            var keyword = SearchKeyword.ToLower();
            var matchingBooks = _books.Where(b => b.Title!.ToLower().Contains(keyword)).ToList();

            if (matchingBooks.Count == 0)
            {
                ResultText = $"Книг с ключевым словом \"{SearchKeyword}\" не найдено.";
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"=== Авторы, книги которых содержат \"{SearchKeyword}\": ===");
            sb.AppendLine();

            var uniqueAuthors = matchingBooks.Select(b => b.Author).Distinct().ToList();

            foreach (var author in uniqueAuthors)
            {
                sb.AppendLine($"Автор: {author}");
                var authorBooks = matchingBooks.Where(b => b.Author == author).ToList();
                foreach (var book in authorBooks)
                {
                    sb.AppendLine($"  • {book.Title} | Тираж: {book.Circulation} | Цена: {book.Price:C} | Год: {book.Year}");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"Всего найдено: {matchingBooks.Count} книг от {uniqueAuthors.Count} авторов.");
            ResultText = sb.ToString();
        }

        private void AddSampleData()
        {
            var sampleBooks = new List<Book>
            {
                new Book { Author = "Агата Кристи", Title = "Убийство в Восточном экспрессе", Circulation = 50000, Price = 450.00m, Year = 1934 },
                new Book { Author = "Агата Кристи", Title = "Убийство Роджера Экройда", Circulation = 45000, Price = 420.00m, Year = 1926 },
                new Book { Author = "Агата Кристи", Title = "Десять негритят", Circulation = 60000, Price = 380.00m, Year = 1939 },
                new Book { Author = "Артур Конан Дойл", Title = "Этюд в багровых тонах", Circulation = 30000, Price = 350.00m, Year = 1887 },
                new Book { Author = "Артур Конан Дойл", Title = "Собака Баскервилей", Circulation = 55000, Price = 400.00m, Year = 1902 },
                new Book { Author = "Дэн Браун", Title = "Код да Винчи", Circulation = 100000, Price = 550.00m, Year = 2003 },
                new Book { Author = "Дэн Браун", Title = "Убийство на улице Морг", Circulation = 25000, Price = 300.00m, Year = 2001 },
                new Book { Author = "Эдгар Аллан По", Title = "Убийство на улице Морг", Circulation = 20000, Price = 280.00m, Year = 1841 },
                new Book { Author = "Эдгар Аллан По", Title = "Золотой жук", Circulation = 15000, Price = 250.00m, Year = 1843 },
                new Book { Author = "Гилберт Кийт Честертон", Title = "Человек, который был Четвергом", Circulation = 18000, Price = 320.00m, Year = 1908 },
            };

            _books.AddRange(sampleBooks);
            ResultText = $"Добавлено {sampleBooks.Count} тестовых книг. Всего книг: {_books.Count}";
        }
    }
}
