# SmartVillages

## Projekt za završni rad koji služi kao lokalna digitalna tržnica

Web aplikacija Smart Villages je veći projekt koji je rađen upotpunosti samostalno u sklopu faksa. Projekt je predan kao završni rad te je kao takav bio i tretiran od samog početka. Od osmišljavanja dizajna i svih funkcionalnosti aplikacije do dizajna baze podataka.

Svrha aplikacije je da prodavači mogu dodavati svoje domaće proizvode na takozvanu "Tržnicu" gdje potom kupci mogu naručivati iste. Nakon što proizvod bude označen kao dostavljen kupcac ga može ocijeniti i komentirati. Također integrirao je i real-time slanje poruka između kupaca i prodavača.

Aplikacija sadrži jednostavan proces _prijave/registracije_ za koje je potrebna verifikacija mailom. Integrirano je _real-time slanje poruka_ uz opcije pročitana/nepročitana.

---

### Built With

- **Blazor - WebAssembly** u ASP.NET Core-u
- **Entity Framework Core** za kreiranje i migracije baze podataka
- **SignalR** za slanje real-time obavijesti(poruka)
- **MudBlazor** - Blazor Component Library za dizajn
- **Microsoft SQL Server** za rad sa bazom podataka

[![My Skills](https://skills.thijs.gg/icons?i=wasm,html,css,js,bootstrap)](https://skills.thijs.gg)

---

## Project Setup

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

te na kraju pokrećemo aplikaciju tako da nam je `Server` jedini `Startup Project`. To je zato jer server projekt ima referencu na klijent projekt i vrte se na istom portu. Potom registriramo korisnika ili poljoprivrednika prateći uputstva.

---

U budućnosti bi trebalo napraviti fajl sa pripremljenim podacima za bazu za `Quick Start`.
