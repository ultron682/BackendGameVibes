## Projekt: Backend dla serwisu społecznościowego poświęconego grą komputerowych

### Opis
Ten backend to kompleksowy system stworzony dla 2 aplikacji klienckich (Flutter i React) który zapewnienia poprawność działania dla całego serwisu gier komputerowych. Łączy funkcjonalności społecznościowe z zarządzaniem bazą gier.

### Pokaz możliwości aplikacji klienckich (flutter - mobile app, react - website for managing app) korzystających z tego backendu
![image](https://github.com/user-attachments/assets/bbd43c03-0166-4c59-b301-39043911c541)
![image](https://github.com/user-attachments/assets/9c8b4285-55d5-43c8-be65-8969216ce75c)
![image](https://github.com/user-attachments/assets/f7d936eb-4586-4848-a9da-726224dd3ce5)
![image](https://github.com/user-attachments/assets/98969b01-6005-4183-883c-f1e0b67d184b)

TODO: zrzuty ekranu


### Cele Projektu
- **Zbudowanie społeczności**: Umożliwienie graczom dzielenia się swoimi doświadczeniami i opiniami na temat gier.
- **Zarządzanie danymi o grach**: Umożliwienie użytkownikom przeglądania, oceniania i recenzowania gier.

### Funkcjonalności Backend
1. **Zarządzanie Użytkownikami**
   - Rejestracja: Użytkownicy mogą tworzyć konta, podając swoje dane osobowe, takie jak imię, nazwisko, adres e-mail oraz hasło. System weryfikuje istnienie adresu e-mail.
   - Logowanie: Umożliwia użytkownikom dostęp do swojego konta z wykorzystaniem bezpiecznego mechanizmu autoryzacji, w tym obsługę błędów logowania.
   - Zarządzanie Sesjami: Obsługuje sesje użytkowników, zapewniając ich bezpieczeństwo i prywatność. Użytkownicy mogą wylogować się z systemu, co kończy ich sesję.

2. **Zarządzanie Grami**
   - CRUD dla Gier: Umożliwia dodawanie, edytowanie i usuwanie gier z bazy danych. Administratorzy mają pełne uprawnienia do zarządzania danymi o grach.
   - Informacje o Gra: Przechowuje szczegółowe informacje o grach, takie jak tytuł, opis, gatunek, data wydania, oceny i recenzje. Użytkownicy mogą przeszukiwać gry według różnych kryteriów.

3. **Recenzje i Oceny**
   - Dodawanie Recenzji: Użytkownicy mogą dodawać recenzje do gier, które obejmują tekst oraz ocenę w skali 1-10. Recenzje są powiązane z kontem użytkownika.
   - Moderacja Treści: Administratorzy mają możliwość przeglądania i moderowania recenzji, aby zapewnić odpowiednie standardy treści. System automatycznie oznacza recenzje, które mogą wymagać moderacji.

4. **Forum Dyskusyjne**
   - Tworzenie Wątków: Użytkownicy mogą zakładać wątki dyskusyjne na forum, dzieląc się swoimi przemyśleniami na temat gier. Wątki mogą być tematyczne i związane z konkretnymi grami.
   - Posty: Umożliwia dodawanie postów do wątków, z opcją oceniania postów przez innych użytkowników. Użytkownicy mogą również odpowiadać na posty, co sprzyja interakcji.

5. **API**
   - RESTful API: Aplikacja udostępnia API do komunikacji z frontendem, umożliwiając operacje CRUD na użytkownikach, grach, recenzjach i postach. API jest dobrze udokumentowane, co ułatwia integrację z frontendem oraz testowanie funkcji.
   - Dokumentacja API: Użycie middleware UseSwagger do generowania dokumentacji API, co ułatwia integrację z frontendem oraz testowanie funkcji.

6. **Middleware**
   - Aktualizacja Aktywności: Middleware aktualizuje ostatnią aktywność użytkownika przy każdym żądaniu, co pozwala na lepsze zarządzanie sesjami i monitorowanie aktywności.


### Baza danych
![image](https://github.com/user-attachments/assets/91938e3e-63ae-45b4-ad41-bd977d69a2fa)


### Swagger
![image](https://github.com/user-attachments/assets/b543afa8-2045-40a0-9ce0-4d197b8d0872)



### Technologie
- **C#**: Język programowania użyty do implementacji logiki backendu, zapewniający wysoką wydajność i bezpieczeństwo.
- **ASP.NET Core**: Framework do budowy aplikacji webowych, który wspiera architekturę MVC oraz RESTful API, co ułatwia rozwój aplikacji.
- **Entity Framework**: ORM (Object-Relational Mapping) do zarządzania bazą danych, co ułatwia operacje na danych oraz migracje.
- **MySQL**: System zarządzania bazą danych, w którym przechowywane są wszystkie dane aplikacji. Umożliwia szybkie i efektywne przetwarzanie zapytań.

### Jak uruchomić projekt
1. **Sklonuj repozytorium**
   ```bash
   git clone https://github.com/ultron682/BackendGameVibes.git
   ```
2. **Zainstaluj wymagane pakiety**
   ```bash
   dotnet restore
   ```
3. **Skonfiguruj bazę danych**: Upewnij się, że masz zainstalowany MySQL i skonfiguruj połączenie w pliku konfiguracyjnym.
4. **Uruchom migracje**: Aby utworzyć bazę danych i tabele, uruchom:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. **Uruchom aplikację**
   ```bash
   dotnet run
   ```

### Wymagania
- **.NET Core**: Wersja 8.0 lub nowsza.
- **MySQL**: Zainstalowany i skonfigurowany przed uruchomieniem migracji.
- **Docker**: Projekt można uruchomić przez docker compose, który automatycznie wszystko uruchomi wraz z bazą danych mysql. 

### Testowanie
- **Testy jednostkowe**: 
- **Testy integracyjne**: 
