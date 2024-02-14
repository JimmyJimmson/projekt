using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        List<Pracownik> pracownicy = new List<Pracownik>
        {
            new PracownikFizyczny(1, "Jan Nowak", new DateTime(2002, 3, 4), 18.5),
            new Urzednik(2, "Agnieszka Kowalska", new DateTime(1973, 12, 15), 2800),
            new PracownikFizyczny(3, "Robert Lewandowski", new DateTime(1980, 5, 23), 29.0),
            new Urzednik(4, "Zofia Plucińska", new DateTime(1998, 11, 2), 4750),
            new PracownikFizyczny(5, "Grzegorz Braun", new DateTime(1960, 1, 29), 48.0),
        };

        while (true)
        {
            Console.WriteLine("WYBIERZ OPCJĘ:\n");
            Console.WriteLine("1 => LISTA WSZYSTKICH PRACOWNIKÓW");
            Console.WriteLine("2 => WYLICZ PENSJĘ PRACOWNIKA");
            Console.WriteLine("3 => ZAKOŃCZ PROGRAM\n");
            Console.Write("WYBIERZ 1, 2 LUB 3: ");

            int opcja = int.Parse(Console.ReadLine());

            switch (opcja)
            {
                case 1:
                    WyswietlPracownikow(pracownicy);
                    break;

                case 2:
                    Console.WriteLine("Podaj Id pracownika:");
                    int idPracownika = int.Parse(Console.ReadLine());

                    Pracownik wybrany = pracownicy.Find(p => p.Id == idPracownika);

                    if (wybrany != null)
                    {
                        Console.WriteLine(wybrany.Szczegoly());

                        int dni;
                        do
                        {
                            Console.WriteLine("Podaj ilość przepracowanych dni w miesiącu (max. 20):");
                            dni = int.Parse(Console.ReadLine());

                            if (dni > 20)
                            {
                                Console.WriteLine("Liczba dni jest za duża. Podaj ponownie.");
                            }
                        } while (dni > 20);

                        Console.WriteLine("Podaj wysokość premii dodatkowej:");
                        double premia = double.Parse(Console.ReadLine());

                        double wynagrodzenie = wybrany.WyliczWynagrodzenie(dni, premia);

                        Console.WriteLine($"Łączne wynagrodzenie brutto: {wynagrodzenie.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}");
                        double podatek = wybrany.Wiek > 26 ? 0.18 * wynagrodzenie : 0;
                        Console.WriteLine($"Podatek: {podatek.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}");
                        Console.WriteLine($"Wynagrodzenie netto do wypłaty: {(wynagrodzenie - podatek).ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}");
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowe Id pracownika. Nie ma takiego pracownika.");
                    }
                    break;

                case 3:
                    Console.WriteLine("Zakończono program.");
                    return;

                default:
                    Console.WriteLine("Nieprawidłowy wybór. Wybierz ponownie.");
                    break;
            }
        }
    }

    static void WyswietlPracownikow(List<Pracownik> pracownicy)
    {
        Console.WriteLine("Lista pracowników:");
        Console.WriteLine("Id\tImię i nazwisko\tData urodzenia\t\t\tStanowisko\tStawka godzinowa/Pensja stała");
        foreach (var pracownik in pracownicy)
        {
            Console.WriteLine($"{pracownik.Id}\t{pracownik.ImieNazwisko}\t{pracownik.DataUrodzenia:dd.MM.yyyy}\t{pracownik.Stanowisko}\t\t{pracownik.StawkaCzyPensja()}");
        }
    }
}

abstract class Pracownik
{
    public int Id { get; }
    public string ImieNazwisko { get; }
    public DateTime DataUrodzenia { get; }
    public int Wiek => DateTime.Now.Year - DataUrodzenia.Year;

    protected Pracownik(int id, string imieNazwisko, DateTime dataUrodzenia)
    {
        Id = id;
        ImieNazwisko = imieNazwisko;
        DataUrodzenia = dataUrodzenia;
    }

    public abstract string Stanowisko { get; }

    public abstract string Szczegoly();

    public abstract double WyliczWynagrodzenie(int przepracowaneDni, double premia);

    public abstract string StawkaCzyPensja();
}

class Urzednik : Pracownik
{
    public double PensjaStala { get; }

    public Urzednik(int id, string imieNazwisko, DateTime dataUrodzenia, double pensjaStala)
        : base(id, imieNazwisko, dataUrodzenia)
    {
        PensjaStala = pensjaStala;
    }

    public override string Stanowisko => "Urzędnik";

    public override string Szczegoly()
    {
        return $"{ImieNazwisko}, Wiek: {Wiek}, Stanowisko: {Stanowisko}, Pensja stała: {PensjaStala.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}";
    }

    public override double WyliczWynagrodzenie(int przepracowaneDni, double premia)
    {
        double podstawa = przepracowaneDni == 20 ? PensjaStala : 0.8 * PensjaStala;
        return podstawa + premia;
    }

    public override string StawkaCzyPensja()
    {
        return PensjaStala == 0 ? "brak" : $"{PensjaStala.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}";
    }
}

class PracownikFizyczny : Pracownik
{
    public double StawkaGodzinowa { get; }

    public PracownikFizyczny(int id, string imieNazwisko, DateTime dataUrodzenia, double stawkaGodzinowa)
        : base(id, imieNazwisko, dataUrodzenia)
    {
        StawkaGodzinowa = stawkaGodzinowa;
    }

    public override string Stanowisko => "Pracownik fizyczny";

    public override string Szczegoly()
    {
        return $"{ImieNazwisko}, Wiek: {Wiek}, Stanowisko: {Stanowisko}, Stawka godzinowa: {StawkaGodzinowa.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}";
    }

    public override double WyliczWynagrodzenie(int przepracowaneDni, double premia)
    {
        double podstawa = przepracowaneDni * StawkaGodzinowa * 8;
        return podstawa + (przepracowaneDni == 20 ? premia : 0);
    }

    public override string StawkaCzyPensja()
    {
        return $"{StawkaGodzinowa.ToString("C", CultureInfo.GetCultureInfo("pl-PL"))}/h";
    }
}

