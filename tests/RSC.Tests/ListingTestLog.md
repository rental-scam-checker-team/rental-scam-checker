# Test log: Listing class (tblListing)

Owner: Aman Paudel · Jira: RSC-24 · Run date: 2026-11-13 · Test file: `tests/RSC.Tests/ListingTests.cs`

Every rule from the spec is tested at its boundaries: min-1, min, min+1, mid, max-1, max, max+1, an extreme value and an invalid type. Valid values used for the other fields: Address "Nørrebrogade 45, 2. th.", Area "Copenhagen", MonthlyRent "6500", SizeM2 "45", PostedDate one week ago, Furnished "false".

**Result: 57 of 57 boundary tests passed.** Together with the 7 other tests (instance, properties, all valid, city spelling, error messages), all 64 tests in ListingTests pass.

| ID | Field | Boundary | Input | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- | --- | --- |
| T01 | Address | min-1 (blank) | 0 characters | Error message | Address must not be blank. | Pass |
| T02 | Address | min | 1 characters | Valid (no error) | Valid (no error) | Pass |
| T03 | Address | min+1 | 2 characters | Valid (no error) | Valid (no error) | Pass |
| T04 | Address | mid | 50 characters | Valid (no error) | Valid (no error) | Pass |
| T05 | Address | max-1 | 99 characters | Valid (no error) | Valid (no error) | Pass |
| T06 | Address | max | 100 characters | Valid (no error) | Valid (no error) | Pass |
| T07 | Address | max+1 | 101 characters | Error message | Address must be 100 characters or less. | Pass |
| T08 | Address | extreme | 500 characters | Error message | Address must be 100 characters or less. | Pass |
| T09 | Area (city) | valid value | "Copenhagen" | Valid (no error) | Valid (no error) | Pass |
| T10 | Area (city) | valid value | "Aarhus" | Valid (no error) | Valid (no error) | Pass |
| T11 | Area (city) | valid value | "Odense" | Valid (no error) | Valid (no error) | Pass |
| T12 | Area (city) | valid value | "Aalborg" | Valid (no error) | Valid (no error) | Pass |
| T13 | Area (city) | capital letters do not matter | "aarhus" | Valid (no error) | Valid (no error) | Pass |
| T14 | Area (city) | spaces are trimmed | " Odense " | Valid (no error) | Valid (no error) | Pass |
| T15 | Area (city) | blank | "" | Error message | City must not be blank. | Pass |
| T16 | Area (city) | a neighbourhood, not a city | "Nørrebro" | Error message | City must be one of: Copenhagen, Aarhus, Odense, Aalborg. | Pass |
| T17 | Area (city) | not a benchmark city | "London" | Error message | City must be one of: Copenhagen, Aarhus, Odense, Aalborg. | Pass |
| T18 | MonthlyRent | min-1 (0 is not allowed) | "0" | Error message | Monthly rent must be greater than 0. | Pass |
| T19 | MonthlyRent | min | "0.01" | Valid (no error) | Valid (no error) | Pass |
| T20 | MonthlyRent | min+1 | "1" | Valid (no error) | Valid (no error) | Pass |
| T21 | MonthlyRent | mid | "25000" | Valid (no error) | Valid (no error) | Pass |
| T22 | MonthlyRent | max-1 | "49999.99" | Valid (no error) | Valid (no error) | Pass |
| T23 | MonthlyRent | max | "50000" | Valid (no error) | Valid (no error) | Pass |
| T24 | MonthlyRent | max+1 | "50000.01" | Error message | Monthly rent must be 50000 kr or less. | Pass |
| T25 | MonthlyRent | negative | "-500" | Error message | Monthly rent must be greater than 0. | Pass |
| T26 | MonthlyRent | extreme | "10000000" | Error message | Monthly rent must be 50000 kr or less. | Pass |
| T27 | MonthlyRent | invalid type | "abc" | Error message | Monthly rent must be a number. | Pass |
| T28 | MonthlyRent | blank | "" | Error message | Monthly rent must be a number. | Pass |
| T29 | MonthlyRent |  | "6,500" | Error message | Monthly rent must be a number. | Pass |
| T30 | SizeM2 | min-1 | "4" | Error message | Size must be between 5 and 500 m². | Pass |
| T31 | SizeM2 | min | "5" | Valid (no error) | Valid (no error) | Pass |
| T32 | SizeM2 | min+1 | "6" | Valid (no error) | Valid (no error) | Pass |
| T33 | SizeM2 | mid | "250" | Valid (no error) | Valid (no error) | Pass |
| T34 | SizeM2 | max-1 | "499" | Valid (no error) | Valid (no error) | Pass |
| T35 | SizeM2 | max | "500" | Valid (no error) | Valid (no error) | Pass |
| T36 | SizeM2 | max+1 | "501" | Error message | Size must be between 5 and 500 m². | Pass |
| T37 | SizeM2 | negative | "-5" | Error message | Size must be between 5 and 500 m². | Pass |
| T38 | SizeM2 | extreme | "100000" | Error message | Size must be between 5 and 500 m². | Pass |
| T39 | SizeM2 | invalid type | "abc" | Error message | Size must be a whole number. | Pass |
| T40 | SizeM2 | not a whole number | "45.5" | Error message | Size must be a whole number. | Pass |
| T41 | PostedDate | extreme past (10 years ago) | today -3650 days (2016-11-15) | Valid (no error) | Valid (no error) | Pass |
| T42 | PostedDate | yesterday | today -1 days (2026-11-12) | Valid (no error) | Valid (no error) | Pass |
| T43 | PostedDate | today (max) | today +0 days (2026-11-13) | Valid (no error) | Valid (no error) | Pass |
| T44 | PostedDate | tomorrow (max+1) | today +1 days (2026-11-14) | Error message | Posted date cannot be in the future. | Pass |
| T45 | PostedDate | extreme future | today +3650 days (2036-11-10) | Error message | Posted date cannot be in the future. | Pass |
| T46 | PostedDate | invalid type | "abc" | Error message | Posted date must be a real date (yyyy-mm-dd). | Pass |
| T47 | PostedDate | no month 13 | "2026-13-01" | Error message | Posted date must be a real date (yyyy-mm-dd). | Pass |
| T48 | PostedDate | no 30 February | "2026-02-30" | Error message | Posted date must be a real date (yyyy-mm-dd). | Pass |
| T49 | PostedDate | blank | "" | Error message | Posted date must be a real date (yyyy-mm-dd). | Pass |
| T50 | Furnished | valid value | "true" | Valid (no error) | Valid (no error) | Pass |
| T51 | Furnished | valid value | "false" | Valid (no error) | Valid (no error) | Pass |
| T52 | Furnished | valid value | "yes" | Valid (no error) | Valid (no error) | Pass |
| T53 | Furnished | valid value | "no" | Valid (no error) | Valid (no error) | Pass |
| T54 | Furnished | valid value | "1" | Valid (no error) | Valid (no error) | Pass |
| T55 | Furnished | valid value | "0" | Valid (no error) | Valid (no error) | Pass |
| T56 | Furnished | invalid value | "maybe" | Error message | Furnished must be yes or no. | Pass |
| T57 | Furnished | invalid value | "" | Error message | Furnished must be yes or no. | Pass |
