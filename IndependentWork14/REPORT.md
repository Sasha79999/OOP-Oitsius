# Звіт: Dictionary і HashSet у відкритих проєктах

## 1. Результати аналізу

### Приклад 1: Dictionary у Dapper

Файл SqlMapper.cs, оголошення словника (рядок 169): https://github.com/StackExchange/Dapper/blob/684d7d9895e8b9eff4ab898528d69bd60979c098/Dapper/SqlMapper.cs#L169

Файл SqlMapper.cs, заповнення словника в статичному конструкторі (рядки 171-214): https://github.com/StackExchange/Dapper/blob/684d7d9895e8b9eff4ab898528d69bd60979c098/Dapper/SqlMapper.cs#L171-L214

У бібліотеці Dapper (це micro-ORM, яка перетворює результати SQL-запитів на об'єкти) у файлі SqlMapper.cs є словник typeMap типу Dictionary<Type, DbType>. Він зіставляє .NET-типи (int, string, DateTime та інші, усього 37 пар) із типами бази даних DbType. Словник заповнюється один раз у статичному конструкторі.

Тут обрано Dictionary, бо треба швидко знайти значення за ключем. Якби це був List, довелося б щоразу перебирати всі пари. Пошук у словнику виконується за O(1) у середньому, а в списку за O(n). У коді пошук робиться через TryGetValue, наприклад у методі LookupDbType. Власний comparer не потрібен, бо ключем є Type, і для нього вистачає звичайного порівняння. Цікава особливість: метод AddTypeMap не змінює словник напряму, а копіює його, змінює копію і підміняє посилання, щоб уникнути проблем із потоками.

### Приклад 2: HashSet у .NET runtime

Файл Distinct.cs, клас DistinctIterator і його поля, серед яких HashSet (рядки 95-100): https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Distinct.cs#L95-L100

Файл Distinct.cs, створення множини і додавання елемента (рядки 124-125): https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Distinct.cs#L124-L125

Файл Distinct.cs, використання HashSet у методі DistinctBy (рядки 78-82): https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Distinct.cs#L78-L82

У .NET runtime у файлі Distinct.cs метод Distinct повертає унікальні елементи послідовності. Клас DistinctIterator має поле HashSet<TSource> і comparer. Елемент повертається тільки тоді, коли його вдалося додати в множину, тобто такого ще не було (у DistinctBy це set.Add(keySelector(element))).

HashSet обрано, бо треба швидко перевіряти, чи елемент уже зустрічався. Якби використали List, кожна перевірка була б O(n), а на всю послідовність вийшло б O(n²). У HashSet додавання і перевірка виконуються за O(1) у середньому, тому весь Distinct працює за O(n). Особливі вимоги тут є: можна передати власний IEqualityComparer, а якщо передати null, береться стандартний.

## 2. Порівняльна таблиця

| Операція | List | Dictionary<TKey,TValue> | HashSet | Коли використовувати |
|---|---|---|---|---|
| Додавання | O(1) | O(1) | O(1) | усі швидкі |
| Пошук за значенням | O(n) | N/A | O(1) | HashSet |
| Пошук за ключем | N/A | O(1) | N/A | Dictionary |
| Видалення | O(n) | O(1) | O(1) | краще не List |
| Перевірка наявності | O(n) | O(1) | O(1) | HashSet або Dictionary |

Для Dictionary і HashSet O(1) це в середньому.

## 3. Висновки про вибір колекції

Dictionary краще за List, коли треба шукати значення за ключем. Наприклад, у кеші метаданих файлів ім'я файлу є ключем. У списку довелося б перебирати всі елементи, а у словнику пошук майже миттєвий.

HashSet найкращий, коли важливі унікальність елементів і швидка перевірка, чи елемент є. Ще він зручний для операцій над множинами. У моєму TagManager теги статті не повторюються, а спільні теги двох статей знаходяться через IntersectWith.

Є кілька підводних каменів. Порядок елементів у Dictionary і HashSet не гарантований. Якщо ключем є власний клас, методи Equals і GetHashCode мають бути узгоджені, інакше елемент не знайдеться. Ключ не можна змінювати після додавання. Колекцію не можна змінювати під час циклу foreach. Також хеш-колекції займають більше пам'яті, ніж List.

## 4. Приклади з власного коду

У проєкті є два класи. FileMetadataCache зберігає метадані файлів у Dictionary і використовує власний comparer StringComparer.OrdinalIgnoreCase, тому report.txt і REPORT.TXT це той самий ключ. TagManager зберігає теги статей у Dictionary<string, HashSet<string>> і використовує UnionWith, IntersectWith та ExceptWith. Вивід програми показано в README.md.