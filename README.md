# HolidayMaker – Bokningssystem

Ett konsolbaserat bokningssystem utvecklat i **C# (.NET)** med **MySQL** som databas. Systemet hanterar kunder, hotell, rum och bokningar med fullständig datapersistens, validering av inmatning samt skydd mot dubbelbokningar och obehöriga ändringar.

---

## Funktioner

* **Kundhantering:**
  * Välj en befintlig kund från databasen.
  * Registrera en ny kund (med validering av för- och efternamn samt e-postadress).
* **Sökning och filtrering av lediga rum:**
  * Sök på ort (t.ex. Halmstad eller Malmö) och datumintervall (incheckning och utcheckning).
  * Filtrera på antal personer, pool, restaurang, maximalt avstånd till strand samt maximalt avstånd till centrum.
  * Sortera sökresultat efter lägsta pris eller högsta omdöme.
  * Automatisk kontroll av rumstillgänglighet för att förhindra överlappande bokningar.
* **Bokningshantering:**
  * Skapa nya bokningar med automatisk beräkning av totalpris baserat på antal nätter och rumspris.
  * Visa aktiva bokningar för den valda kunden.
  * Uppdatera antal personer i en befintlig bokning (med kontroll av rummets maxkapacitet samt kundägarskap).
  * Avboka (ta bort) en bokning (begränsat till den valda kundens egna bokningar).

---

## Projektstruktur

Projektet är uppdelat i tydliga lager för god struktur och underhållbarhet:

```text
HolidayMaker/
├── Data/
│ └── Database.cs # Hanterar databasanslutning till MySQL
├── Models/
│ ├── Booking.cs # Datamodell för bokning
│ ├── BookingInfo.cs # Datamodell för visning av bokningsinformation
│ ├── Customer.cs # Datamodell för kund
│ ├── Hotel.cs # Datamodell för hotell
│ ├── Room.cs # Datamodell för rum
│ └── RoomSearchResult.cs # Datamodell för sökresultat av rum
├── Repositories/
│ ├── BookingRepository.cs # SQL-frågor för skapande, uppdatering och borttagning av bokningar
│ ├── CustomerRepository.cs # SQL-frågor för kunder
│ ├── HotelRepository.cs # SQL-frågor för hotell
│ └── RoomRepository.cs # SQL-frågor och filtrering för rum
├── HolidayMaker.sql # SQL-skript för att skapa tabeller och testdata
├── Program.cs # Konsolgränssnitt, menysystem och inmatningsvalidering
├── HolidayMaker.csproj # Projektfil och paketberoenden (MySqlConnector)
└── README.md # Projektdokumentation

Databasdesign
Databasen definieras i HolidayMaker.sql och består av följande tabeller:
 * Customers: Lagrar kunduppgifter (id, name, address, email, phone).
 * Hotels: Lagrar hotellinformation (id, name, location, has_pool, has_restaurant, distance_to_beach, distance_to_center, rating).
 * Rooms: Lagrar hotellrum kopplade till respektive hotell (id, hotel_id, room_number, capacity, price_per_night).
 * Bookings: Lagrar bokningar kopplade till kund och rum (id, customer_id, room_id, check_in, check_out, num_guests, total_price).
Säkerhet och validering
 * SQL Injection-skydd: Alla databasfrågor använder parametriserade frågor (Parameters.AddWithValue).
 * Inmatningsvalidering: Kontroll av datumformat (YYYY-MM-DD), att utcheckning sker efter incheckning, att antal personer är ett giltigt positivt heltal samt skydd mot ogiltig textinmatning.
 * Affärsregler i databasen: Uppdatering och avbokning kontrollerar både customer_id och rummets capacity direkt i SQL-frågan.
Kom igång och kör projektet
Förutsättningar
 * .NET SDK (8.0 eller senare)
 * En körande MySQL-server
Installation och körning
 * Klona repot:
   git clone <länk-till-ditt-repo>
cd HolidayMaker

 * Kör SQL-skriptet HolidayMaker.sql i din MySQL-server för att skapa databasen, tabellerna och initial testdata.
 * Kontrollera och uppdatera anslutningssträngen (lösenord/server) i Data/Database.cs vid behov.
 * Kör applikationen:
   dotnet run
