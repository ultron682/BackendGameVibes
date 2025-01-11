Projekt: System dla Miłośników Gier Komputerowych
Opis
Ten projekt to kompleksowy system stworzony z myślą o miłośnikach gier komputerowych, który łączy funkcjonalności społecznościowe z zarządzaniem bazą gier. Aplikacja umożliwia użytkownikom interakcję, dzielenie się opiniami oraz odkrywanie nowych gier w przyjaznym i intuicyjnym środowisku. Backend aplikacji został zbudowany przy użyciu technologii C# i ASP.NET Core, co zapewnia wysoką wydajność, bezpieczeństwo oraz łatwość w rozwoju i utrzymaniu.

Cele Projektu
Zbudowanie społeczności: Umożliwienie graczom dzielenia się swoimi doświadczeniami i opiniami na temat gier.

Zarządzanie danymi o grach: Umożliwienie użytkownikom przeglądania, oceniania i recenzowania gier.

Intuicyjny interfejs: Zapewnienie prostego i przyjaznego interfejsu, który ułatwia nawigację i korzystanie z aplikacji.

Funkcjonalności Backend
1. Zarządzanie Użytkownikami
Rejestracja: Użytkownicy mogą tworzyć konta, podając swoje dane osobowe, takie jak imię, nazwisko, adres e-mail oraz hasło. System weryfikuje unikalność adresu e-mail.

Logowanie: Umożliwia użytkownikom dostęp do swojego konta z wykorzystaniem bezpiecznego mechanizmu autoryzacji, w tym obsługę błędów logowania.

Zarządzanie Sesjami: Obsługuje sesje użytkowników, zapewniając ich bezpieczeństwo i prywatność. Użytkownicy mogą wylogować się z systemu, co kończy ich sesję.

2. Zarządzanie Grami
CRUD dla Gier: Umożliwia dodawanie, edytowanie i usuwanie gier z bazy danych. Administratorzy mają pełne uprawnienia do zarządzania danymi o grach.

Informacje o Gra: Przechowuje szczegółowe informacje o grach, takie jak tytuł, opis, gatunek, data wydania, oceny i recenzje. Użytkownicy mogą przeszukiwać gry według różnych kryteriów.

3. Recenzje i Oceny
Dodawanie Recenzji: Użytkownicy mogą dodawać recenzje do gier, które obejmują tekst oraz ocenę w skali 1-10. Recenzje są powiązane z kontem użytkownika.

Moderacja Treści: Administratorzy mają możliwość przeglądania i moderowania recenzji, aby zapewnić odpowiednie standardy treści. System automatycznie oznacza recenzje, które mogą wymagać przeglądu.

4. Forum Dyskusyjne
Tworzenie Wątków: Użytkownicy mogą zakładać wątki dyskusyjne na forum, dzieląc się swoimi przemyśleniami na temat gier. Wątki mogą być tematyczne i związane z konkretnymi grami.

Posty: Umożliwia dodawanie postów do wątków, z opcją oceniania postów przez innych użytkowników. Użytkownicy mogą również odpowiadać na posty, co sprzyja interakcji.

5. API
RESTful API: Aplikacja udostępnia API do komunikacji z frontendem, umożliwiając operacje CRUD na użytkownikach, grach, recenzjach i postach. API jest dobrze udokumentowane, co ułatwia integrację.

Dokumentacja API: Użycie middleware UseSwagger do generowania dokumentacji API, co ułatwia integrację z frontendem oraz testowanie funkcji.

6. Middleware
CORS: Obsługuje politykę CORS, co pozwala na bezpieczne połączenia z różnych domen. Umożliwia to korzystanie z aplikacji w różnych środowiskach.

Aktualizacja Aktywności: Middleware aktualizuje ostatnią aktywność użytkownika przy każdym żądaniu, co pozwala na lepsze zarządzanie sesjami i monitorowanie aktywności.

Technologie
C#: Język programowania użyty do implementacji logiki backendu, zapewniający wysoką wydajność i bezpieczeństwo.

ASP.NET Core: Framework do budowy aplikacji webowych, który wspiera architekturę MVC oraz RESTful API, co ułatwia rozwój aplikacji.

Entity Framework: ORM (Object-Relational Mapping) do zarządzania bazą danych, co ułatwia operacje na danych oraz migracje.

MySQL: System zarządzania bazą danych, w którym przechowywane są wszystkie dane aplikacji. Umożliwia szybkie i efektywne przetwarzanie zapytań.

Jak uruchomić projekt
Sklonuj repozytorium:

Zainstaluj wymagane pakiety:

Skonfiguruj bazę danych: Upewnij się, że masz zainstalowany MySQL i skonfiguruj połączenie w pliku . Przykładowa konfiguracja:

Uruchom migracje: Aby utworzyć bazę danych i tabele, uruchom:

Uruchom aplikację:

Wymagania
.NET Core: Wersja 5.0 lub nowsza. Upewnij się, że masz zainstalowane odpowiednie SDK.

MySQL: Zainstalowany i skonfigurowany system zarządzania bazą danych. Upewnij się, że masz utworzoną bazę danych przed uruchomieniem migracji.

Testowanie
Testy jednostkowe: Projekt zawiera testy jednostkowe dla kluczowych funkcji backendu, co zapewnia stabilność i niezawodność aplikacji.

Testy integracyjne: Umożliwiają sprawdzenie interakcji między różnymi komponentami systemu, co jest kluczowe dla zapewnienia poprawności działania aplikacji.
