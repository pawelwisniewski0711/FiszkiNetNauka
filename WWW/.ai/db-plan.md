# Schemat bazy danych PostgreSQL dla FiszkiNet

## 1. Lista tabel z ich kolumnami, typami danych i ograniczeniami

### AspNetUsers (rozszerzenie ASP.NET Core Identity)
| Kolumna | Typ danych | Ograniczenia | Opis |
|---------|------------|--------------|------|
| Id | varchar(450) | PRIMARY KEY | Unikalny identyfikator użytkownika |
| UserName | varchar(256) | UNIQUE, NOT NULL | Nazwa użytkownika |
| NormalizedUserName | varchar(256) | UNIQUE, NOT NULL | Znormalizowana nazwa użytkownika (do wyszukiwania) |
| Email | varchar(256) | NOT NULL | Adres email użytkownika |
| NormalizedEmail | varchar(256) | NOT NULL | Znormalizowany adres email (do wyszukiwania) |
| EmailConfirmed | boolean | NOT NULL, DEFAULT false | Flaga potwierdzenia adresu email |
| PasswordHash | varchar(max) | NOT NULL | Zahaszowane hasło użytkownika |
| SecurityStamp | varchar(max) | NOT NULL | Znacznik bezpieczeństwa |
| ConcurrencyStamp | varchar(max) | NULL | Znacznik współbieżności |
| PhoneNumber | varchar(50) | NULL | Numer telefonu użytkownika |
| PhoneNumberConfirmed | boolean | NOT NULL, DEFAULT false | Flaga potwierdzenia numeru telefonu |
| TwoFactorEnabled | boolean | NOT NULL, DEFAULT false | Flaga włączenia uwierzytelniania dwuskładnikowego |
| LockoutEnd | timestamp with time zone | NULL | Data zakończenia blokady konta |
| LockoutEnabled | boolean | NOT NULL, DEFAULT false | Flaga możliwości blokady konta |
| AccessFailedCount | integer | NOT NULL, DEFAULT 0 | Liczba nieudanych prób logowania |

*Uwaga: Tabela zawiera standardowe pola ASP.NET Core Identity. Zgodnie z notatkami, nie dodajemy dodatkowych pól specyficznych dla aplikacji w MVP.*

### Flashcards
| Kolumna | Typ danych | Ograniczenia | Opis |
|---------|------------|--------------|------|
| Id | uuid | PRIMARY KEY, DEFAULT gen_random_uuid() | Unikalny identyfikator fiszki |
| UserId | varchar(450) | FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE, NOT NULL | Identyfikator właściciela fiszki |
| Front | varchar(300) | NOT NULL | Przednia strona fiszki (pytanie) |
| Back | varchar(500) | NOT NULL | Tylna strona fiszki (odpowiedź) |
| CreatedAt | timestamp with time zone | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Data utworzenia fiszki |
| UpdatedAt | timestamp with time zone | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Data ostatniej aktualizacji fiszki |
| IsGeneratedByAI | boolean | NOT NULL, DEFAULT false | Flaga określająca, czy fiszka została wygenerowana przez AI |
| EaseFactor | decimal(5,2) | NOT NULL, DEFAULT 2.5 | Współczynnik łatwości dla algorytmu spaced repetition |
| Interval | integer | NOT NULL, DEFAULT 0 | Interwał w dniach do następnej powtórki |
| NextReviewDate | timestamp with time zone | NULL | Data następnej powtórki |
| LastReviewDate | timestamp with time zone | NULL | Data ostatniej powtórki |
| ReviewCount | integer | NOT NULL, DEFAULT 0 | Liczba przeprowadzonych powtórek |

### StudySessions
| Kolumna | Typ danych | Ograniczenia | Opis |
|---------|------------|--------------|------|
| Id | uuid | PRIMARY KEY, DEFAULT gen_random_uuid() | Unikalny identyfikator sesji nauki |
| UserId | varchar(450) | FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE, NOT NULL | Identyfikator użytkownika |
| StartedAt | timestamp with time zone | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Data rozpoczęcia sesji |
| EndedAt | timestamp with time zone | NULL | Data zakończenia sesji |
| FlashcardsReviewed | integer | NOT NULL, DEFAULT 0 | Liczba przeglądniętych fiszek w sesji |

### StudySessionDetails
| Kolumna | Typ danych | Ograniczenia | Opis |
|---------|------------|--------------|------|
| Id | uuid | PRIMARY KEY, DEFAULT gen_random_uuid() | Unikalny identyfikator szczegółów sesji |
| SessionId | uuid | FOREIGN KEY REFERENCES StudySessions(Id) ON DELETE CASCADE, NOT NULL | Identyfikator sesji nauki |
| FlashcardId | uuid | FOREIGN KEY REFERENCES Flashcards(Id) ON DELETE CASCADE, NOT NULL | Identyfikator fiszki |
| PerformanceRating | integer | NOT NULL | Ocena znajomości fiszki (0-5) |
| ReviewedAt | timestamp with time zone | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Data przeglądu fiszki |
| PreviousEaseFactor | decimal(5,2) | NOT NULL | Poprzedni współczynnik łatwości |
| PreviousInterval | integer | NOT NULL | Poprzedni interwał |
| NewEaseFactor | decimal(5,2) | NOT NULL | Nowy współczynnik łatwości |
| NewInterval | integer | NOT NULL | Nowy interwał |

### GenerationStatistics
| Kolumna | Typ danych | Ograniczenia | Opis |
|---------|------------|--------------|------|
| Id | uuid | PRIMARY KEY, DEFAULT gen_random_uuid() | Unikalny identyfikator statystyki |
| UserId | varchar(450) | FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE, NOT NULL | Identyfikator użytkownika |
| GeneratedAt | timestamp with time zone | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Data generowania fiszek |
| TotalGenerated | integer | NOT NULL, DEFAULT 0 | Liczba wygenerowanych fiszek |
| TotalAccepted | integer | NOT NULL, DEFAULT 0 | Liczba zaakceptowanych fiszek |
| SourceTextLength | integer | NOT NULL, DEFAULT 0 | Długość tekstu źródłowego |

## 2. Relacje między tabelami

1. **AspNetUsers (1) -> (N) Flashcards**
   - Jeden użytkownik może mieć wiele fiszek
   - Każda fiszka należy do dokładnie jednego użytkownika

2. **AspNetUsers (1) -> (N) StudySessions**
   - Jeden użytkownik może mieć wiele sesji nauki
   - Każda sesja nauki należy do dokładnie jednego użytkownika

3. **AspNetUsers (1) -> (N) GenerationStatistics**
   - Jeden użytkownik może mieć wiele statystyk generowania
   - Każda statystyka generowania należy do dokładnie jednego użytkownika

4. **StudySessions (1) -> (N) StudySessionDetails**
   - Jedna sesja nauki może mieć wiele szczegółów (przeglądów fiszek)
   - Każdy szczegół sesji należy do dokładnie jednej sesji nauki

5. **Flashcards (1) -> (N) StudySessionDetails**
   - Jedna fiszka może być przeglądana w wielu sesjach nauki
   - Każdy szczegół sesji odnosi się do dokładnie jednej fiszki

## 3. Indeksy

### AspNetUsers
- Indeks na NormalizedUserName (tworzony automatycznie przez ASP.NET Core Identity)
- Indeks na NormalizedEmail (tworzony automatycznie przez ASP.NET Core Identity)

### Flashcards
```sql
CREATE INDEX idx_flashcards_user_id ON Flashcards(UserId);
CREATE INDEX idx_flashcards_next_review_date ON Flashcards(UserId, NextReviewDate);
CREATE INDEX idx_flashcards_created_at ON Flashcards(UserId, CreatedAt);
```

### StudySessions
```sql
CREATE INDEX idx_study_sessions_user_id ON StudySessions(UserId);
CREATE INDEX idx_study_sessions_started_at ON StudySessions(UserId, StartedAt);
```

### StudySessionDetails
```sql
CREATE INDEX idx_study_session_details_session_id ON StudySessionDetails(SessionId);
CREATE INDEX idx_study_session_details_flashcard_id ON StudySessionDetails(FlashcardId);
```

### GenerationStatistics
```sql
CREATE INDEX idx_generation_statistics_user_id ON GenerationStatistics(UserId);
CREATE INDEX idx_generation_statistics_generated_at ON GenerationStatistics(UserId, GeneratedAt);
```

## 4. Zasady PostgreSQL dla Row-Level Security (RLS)

```sql
-- Włączenie RLS dla tabeli Flashcards
ALTER TABLE Flashcards ENABLE ROW LEVEL SECURITY;

-- Polityka dostępu dla Flashcards - użytkownicy mogą widzieć tylko swoje fiszki
CREATE POLICY flashcards_user_policy ON Flashcards
    USING (UserId = current_user_id());

-- Włączenie RLS dla tabeli StudySessions
ALTER TABLE StudySessions ENABLE ROW LEVEL SECURITY;

-- Polityka dostępu dla StudySessions - użytkownicy mogą widzieć tylko swoje sesje
CREATE POLICY study_sessions_user_policy ON StudySessions
    USING (UserId = current_user_id());

-- Włączenie RLS dla tabeli StudySessionDetails
ALTER TABLE StudySessionDetails ENABLE ROW LEVEL SECURITY;

-- Polityka dostępu dla StudySessionDetails - użytkownicy mogą widzieć tylko szczegóły swoich sesji
CREATE POLICY study_session_details_user_policy ON StudySessionDetails
    USING (SessionId IN (SELECT Id FROM StudySessions WHERE UserId = current_user_id()));

-- Włączenie RLS dla tabeli GenerationStatistics
ALTER TABLE GenerationStatistics ENABLE ROW LEVEL SECURITY;

-- Polityka dostępu dla GenerationStatistics - użytkownicy mogą widzieć tylko swoje statystyki
CREATE POLICY generation_statistics_user_policy ON GenerationStatistics
    USING (UserId = current_user_id());

-- Funkcja pomocnicza do uzyskiwania ID bieżącego użytkownika
CREATE OR REPLACE FUNCTION current_user_id()
RETURNS varchar AS $$
BEGIN
    RETURN current_setting('app.current_user_id', true);
EXCEPTION
    WHEN OTHERS THEN
        RETURN NULL;
END;
$$ LANGUAGE plpgsql;
```

## 5. Dodatkowe uwagi i wyjaśnienia

1. **Wybór UUID jako klucza głównego** - Zastosowano UUID jako klucz główny dla większości tabel (poza AspNetUsers, która używa formatu narzuconego przez ASP.NET Core Identity). UUID pozwala na łatwą skalowalność i uniknięcie problemów z sekwencjami w środowisku rozproszonym.

2. **Metadane algorytmu spaced repetition** - Zgodnie z notatkami, metadane algorytmu spaced repetition są przechowywane bezpośrednio w tabeli Flashcards. Domyślne wartości są oparte na algorytmie SM-2 (SuperMemo 2), który jest często używany w aplikacjach do nauki z wykorzystaniem fiszek.

3. **Tabela StudySessionDetails** - Dodano tabelę StudySessionDetails, która przechowuje szczegółowe informacje o każdym przeglądzie fiszki w ramach sesji nauki. Pozwala to na śledzenie postępów użytkownika i dostosowanie algorytmu spaced repetition.

4. **Row-Level Security (RLS)** - Zaimplementowano RLS zgodnie z zaleceniami z notatek, aby zapewnić, że użytkownicy mają dostęp tylko do własnych danych. Wymaga to ustawienia zmiennej sesji 'app.current_user_id' przy każdym zapytaniu.

5. **Indeksy** - Dodano indeksy dla najczęściej używanych kolumn i zapytań, takich jak wyszukiwanie fiszek do powtórki dla danego użytkownika.

6. **Zgodność z Entity Framework Core** - Schemat jest zgodny z konwencjami nazewnictwa Entity Framework Core, co ułatwi implementację migracji i mapowanie obiektowo-relacyjne.

7. **Skalowalność** - Brak limitów na liczbę fiszek per użytkownik, ale indeksy i struktura bazy danych są zoptymalizowane pod kątem wydajności nawet przy dużej liczbie rekordów.

8. **Audyt i RODO** - Wszystkie tabele zawierają znaczniki czasowe utworzenia/aktualizacji, co ułatwia audyt i zapewnienie zgodności z RODO. Dodatkowo, kaskadowe usuwanie zapewnia, że usunięcie konta użytkownika spowoduje usunięcie wszystkich powiązanych danych.
