# Plan bazy danych dla FiszkiNet MVP

## Podjęte decyzje
1. System bazodanowy będzie używać Entity Framework Core 8 z migracjami do zarządzania schematem bazy danych.
2. Dane użytkownika będą ograniczone do adresu email i hasła, zarządzane przez ASP.NET Core Identity.
3. Fiszki będą przechowywane jako pojedyncze elementy przypisane do użytkownika (bez grupowania w zestawy/kolekcje).
4. Fiszka składa się z dwóch stron: front (maksymalnie 300 znaków) i back (maksymalnie 500 znaków).
5. Nie ma potrzeby przechowywania oryginalnego tekstu źródłowego, z którego wygenerowano fiszki.
6. Dane statystyczne dotyczące generowania fiszek nie mają terminu ważności.
7. Nie ma limitów dotyczących liczby fiszek, które użytkownik może utworzyć.
8. Fiszki nie będą miały kategorii ani tagów.
9. Wszystkie fiszki będą wyłącznie tekstowe.
10. Nie ma potrzeby implementacji funkcji eksportu fiszek.

## Rekomendacje
1. Zastosować ASP.NET Core Identity jako bazę dla modelu użytkownika, rozszerzając go tylko o niezbędne pola specyficzne dla aplikacji.
2. Zaimplementować model danych z następującymi głównymi encjami: User, Flashcard, StudySession, GenerationStatistics.
3. Zaprojektować relację jeden-do-wielu między User a Flashcard, gdzie każda fiszka należy do jednego użytkownika.
4. Zastosować Row-Level Security (RLS) dla fiszek, aby zapewnić, że użytkownicy mają dostęp tylko do własnych danych.
5. Przechowywać metadane algorytmu spaced repetition jako część modelu Flashcard (np. EaseFactor, Interval, NextReviewDate).
6. Przechowywać informacje o pochodzeniu fiszki (ręcznie utworzona vs. wygenerowana przez AI).
7. Używać migracji Entity Framework Core do zarządzania ewolucją schematu bazy danych.
8. Zaimplementować mechanizm audytu dla operacji CRUD na fiszkach w celu zapewnienia zgodności z RODO.

## Schemat bazy danych

Na podstawie przeprowadzonej rozmowy i analizy wymagań, schemat bazy danych dla MVP aplikacji FiszkiNet powinien być zbudowany wokół następujących głównych encji:

### User (wykorzystujący ASP.NET Core Identity)
- Podstawowe pola Identity (Id, Email, PasswordHash)
- Bez dodatkowych pól specyficznych dla aplikacji w MVP

### Flashcard
- Id (klucz główny)
- UserId (klucz obcy do tabeli Users)
- Front (tekst, max 300 znaków)
- Back (tekst, max 500 znaków)
- CreatedAt (data utworzenia)
- UpdatedAt (data ostatniej aktualizacji)
- IsGeneratedByAI (flaga określająca źródło fiszki)
- Metadane dla algorytmu spaced repetition:
  - EaseFactor
  - Interval
  - NextReviewDate
  - LastReviewDate

### StudySession
- Id (klucz główny)
- UserId (klucz obcy do tabeli Users)
- StartedAt (data rozpoczęcia sesji)
- EndedAt (data zakończenia sesji)
- FlashcardsReviewed (liczba przeglądniętych fiszek)

### GenerationStatistics
- Id (klucz główny)
- UserId (klucz obcy do tabeli Users)
- GeneratedAt (data generowania)
- TotalGenerated (liczba wygenerowanych fiszek)
- TotalAccepted (liczba zaakceptowanych fiszek)
- SourceTextLength (długość tekstu źródłowego)

## Relacje między encjami
- Jeden użytkownik może mieć wiele fiszek (1:N)
- Jeden użytkownik może mieć wiele sesji nauki (1:N)
- Jeden użytkownik może mieć wiele statystyk generowania (1:N)

## Kluczowe aspekty bezpieczeństwa i skalowalności
- Zastosowanie Row-Level Security (RLS) dla zapewnienia, że użytkownicy mają dostęp tylko do własnych fiszek
- Wykorzystanie ASP.NET Core Identity do bezpiecznego zarządzania użytkownikami i autentykacji
- Brak limitów na liczbę fiszek per użytkownik, ale monitorowanie wzrostu danych
- Przechowywanie metadanych dla algorytmu spaced repetition bezpośrednio w tabeli Flashcard dla uproszczenia MVP
- Przechowywanie informacji o pochodzeniu fiszki (ręcznie utworzona vs. wygenerowana przez AI) dla celów statystycznych

## Nierozwiązane kwestie
1. Nie określono konkretnego algorytmu spaced repetition, który zostanie zaimplementowany (np. SuperMemo, Anki, Leitner).
2. Nie sprecyzowano dokładnie, jakie metadane są wymagane dla wybranego algorytmu spaced repetition.
3. Brak decyzji odnośnie do konkretnego systemu bazodanowego (PostgreSQL, SQL Server), choć wiadomo, że będzie używany Entity Framework Core 8.
4. Nie określono szczegółów dotyczących implementacji statystyk nauki dla użytkownika.
5. Nie sprecyzowano, jak dokładnie powinny być mierzone metryki sukcesu w kontekście struktury bazy danych.
