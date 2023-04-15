# SmartVillages

## Web aplikacija digitalna tržnica

Web aplikacija Smart Villages je veći projekt koji je rađen upotpunosti samostalno u sklopu faksa. Projekt je predan kao završni rad te je kao takav bio i tretiran od samog početka. Od osmišljavanja dizajna i svih funkcionalnosti aplikacije do dizajna baze podataka.

Svrha aplikacije je da prodavači mogu dodavati svoje domaće proizvode na takozvanu "Tržnicu" gdje potom kupci mogu naručivati iste. Nakon što proizvod bude označen kao dostavljen kupac ga može ocijeniti i komentirati. Također integrirao je i real-time slanje poruka između kupaca i prodavača.

### Tehnologije korištene za izradu:

- **Blazor - WebAssembly** u ASP.NET Core-u
- **Entity Framework Core** za kreiranje i migracije baze podataka
- **SignalR** za slanje real-time obavijesti i poruka
- **MudBlazor** - Blazor Component Library za dizajn
- **Microsoft SQL Server** za rad sa bazom podataka

[![My Skills](https://skills.thijs.gg/icons?i=wasm,dotnet,js,bootstrap,sqlite)](https://skills.thijs.gg)

Aplikacija sadrži jednostavan proces _prijave/registracije_ za koje je potrebna verifikacija mailom. Integrirano je _real-time slanje poruka_ uz opcije pročitana/nepročitana.

---

### Kada se poljoprivrednik prijavi u aplikaciju može:

- pretražiti i sortirati tržnicu
- dodavati nove proizvode u tržnicu
- naručiti proizvode drugih poljoprivrednika iz tržnice
- pregledati košaricu
- ažurirati i obrisati svoje proizvode
- pregledati svoje narudžbe, narudžbe drugih ljudi za njegove proizvode i završene naružbe
- postaviti narudžbe svojih proizvoda kao dostavljene
- za svoje završene narudžbe može dati ocjenu i komentar
- slati poruke kupcima njegovih proizvoda
- pregledati svoj profil
- izmjeniti korisničke podatke za svoj profil

### Kada se obićan korisnik prijavi u aplikaciju može:

- pretražiti i sortirati tržnicu
- naručiti proizvode iz tržnice
- pregledati košaricu
- pregledati svoje u trijeku i završene narudžbe
- za svoje završene narudžbe može dati ocjenu i komentar
- slati poruke prodavačima proizvoda
- pregledati svoj profil
- izmjeniti korisničke podatke za svoj profil

---

### Pokretanje projekta

Potrebno je kreirati bazu i pokrenuti migracije na nju preko `Package Manager Console` gdje pokrećemo sljedeću liniju koda.

```sh
update-database
```

Zatim u tablicu `UserTypes` unosimo:

| UserTypeId | UserTypeName    |
| ---------- | --------------- |
| 1          | Korisnik        |
| 2          | Poljoprivrednik |

te u tablicu `Places` unosimo neko mjesto po želji:

| Id  | Name       | PostalCode |
| --- | ---------- | ---------- |
| 1   | Virovitica | 33000      |

- Kako bi se izbjeglo ovakvo unošenje podataka u tablice, trebalo bi napraviti sql datoteku sa pripremljenim podacima za bazu (_Quick Start_) u budućnosti.

Na kraju pokrećemo aplikaciju tako da nam je `Server` jedini `Startup Project`. To je zato jer `Server` projekt ima referencu na `Klijent` projekt i "vrte" se na istom portu. Potom registriramo korisnika ili poljoprivrednika prateći uputstva.
