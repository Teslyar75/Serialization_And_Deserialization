/*Разработать класс "Счет для оплаты". В классе предусмотреть следующие поля:
*оплата за день,
*количество дней,
*штраф за один день оплаты,
*количество дней задержки оплаты,
*сумма к оплате без штрафа(вычисляемое поле),
*штраф (вычисляемое поле),
*общая сумма к оплате(вычисляемое поле).
В классе объявить статическое свойство типа bool, 
значение которое влияет на процесс формирования объектов этого класса. 
Если значение этого свойства  равно true, 
тогда сериализуются и десериализуются все поля, 
если false, тогда вычисляемые поля не сериализуются.
Разработать приложение, в котором необходимо продемонстрировать использование этого класса, 
результаты должны записываться и считываться из файла.*/
using System;
using System.IO;
using System.Xml.Serialization;

public class Payment_Bill
{
    public static bool SerializeAllFields { get; set; }

    public int Вays_Of_Delay { get; set; }
    public double Payment_For_Day { get; set; }
    public double Penalty_For_Day { get; set; }
    public int How_Many_Days { get; set; }

    [XmlIgnore]
    public double Money_Without_Penalty
    {
        get { return Payment_For_Day * How_Many_Days; }
    }

    [XmlIgnore]
    public double Penalty
    {
        get { return Penalty_For_Day * Вays_Of_Delay; }
    }

    [XmlIgnore]
    public double Money_To_Pay
    {
        get { return Money_Without_Penalty + Penalty; }
    }

    public void Serialize(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Payment_Bill));

        using (TextWriter writer = new StreamWriter(fileName))
        {
            if (SerializeAllFields)
            {
                serializer.Serialize(writer, this);
            }
            else
            {
                // Создаем новый объект Payment_Bill с учетом только несериализуемых полей
                Payment_Bill tempBill = new Payment_Bill
                {
                    Payment_For_Day = Payment_For_Day,
                    How_Many_Days = How_Many_Days
                    // Добавьте другие поля по необходимости
                };

                serializer.Serialize(writer, tempBill);
            }

            Console.WriteLine($"\nОбъект успешно сериализован и сохранен в файл {fileName}");
        }
    }

    public static Payment_Bill Deserialize(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Payment_Bill));

        try
        {
            using (TextReader reader = new StreamReader(fileName))
            {
                Payment_Bill loaded_Bill = (Payment_Bill)serializer.Deserialize(reader);
                Console.WriteLine("\nОбъект успешно загружен из файла и десериализован.");
                return loaded_Bill;
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Файл не найден.");
            return null;
        }
    }
}

class Program
{
    static void Main()
    {
        Payment_Bill p_bill = new Payment_Bill();

        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Ввод информации о счете для оплаты");
            Console.WriteLine("2. Вывод информации о счете для оплаты");
            Console.WriteLine("3. Сериализация и сохранение в файл");
            Console.WriteLine("4. Загрузка из файла и десериализация");
            Console.WriteLine("5. Установить/снять флаг сериализации всех полей");
            Console.WriteLine("6. Выход");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Input_Payment_Bill_Info(p_bill);
                    break;
                case "2":
                    Print_Payment_Bill_Info(p_bill);
                    break;
                case "3":
                    Serialize_And_Save(p_bill);
                    break;
                case "4":
                    Deserialize_From_File(p_bill);
                    break;
                case "5":
                    Toggle_Serialization_Flag();
                    break;
                case "6":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте еще раз.");
                    break;
            }
        }
    }

    static void Input_Payment_Bill_Info(Payment_Bill p_bill)
    {
        try
        {
            Console.Write("Введите оплату за день: ");
            p_bill.Payment_For_Day = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введите количество дней: ");
            p_bill.How_Many_Days = Convert.ToInt32(Console.ReadLine());

            Console.Write("Введите штраф за один день оплаты: ");
            p_bill.Penalty_For_Day = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введите количество дней задержки оплаты: ");
            p_bill.Вays_Of_Delay = Convert.ToInt32(Console.ReadLine());
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка в формате введенных данных. Попробуйте еще раз.");
        }
    }

    static void Print_Payment_Bill_Info(Payment_Bill p_bill)
    {
        Console.WriteLine($"\nИнформация о счете для оплаты:");
        Console.WriteLine($"Оплата за день: {p_bill.Payment_For_Day}");
        Console.WriteLine($"Количество дней: {p_bill.How_Many_Days}");
        Console.WriteLine($"Штраф за один день оплаты: {p_bill.Penalty_For_Day}");
        Console.WriteLine($"Количество дней задержки оплаты: {p_bill.Вays_Of_Delay}");
        Console.WriteLine($"Сумма к оплате без штрафа: {p_bill.Money_Without_Penalty}");
        Console.WriteLine($"Штраф: {p_bill.Penalty}");
        Console.WriteLine($"Общая сумма к оплате: {p_bill.Money_To_Pay}");
    }

    static void Serialize_And_Save(Payment_Bill p_bill)
    {
        Console.Write("Введите имя файла для сохранения: ");
        string fileName = Console.ReadLine();
        p_bill.Serialize(fileName);
    }

    static void Deserialize_From_File(Payment_Bill p_bill)
    {
        Console.Write("Введите имя файла для загрузки: ");
        string fileName = Console.ReadLine();
        Payment_Bill loaded_Bill = Payment_Bill.Deserialize(fileName);

        if (loaded_Bill != null)
        {
            Print_Payment_Bill_Info(loaded_Bill);
        }
    }

    static void Toggle_Serialization_Flag()
    {
        Payment_Bill.SerializeAllFields = !Payment_Bill.SerializeAllFields;
        Console.WriteLine($"Флаг сериализации всех полей установлен в {Payment_Bill.SerializeAllFields}");
    }
}


