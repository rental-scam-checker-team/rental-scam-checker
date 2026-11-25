# Test log: SafetyTip (tblSafetyTip)

Owner: Ambika Pandey · Jira: RSC-49 · Run date: 2026-11-19 · Test file: `tests/RSC.Tests/SafetyTipTests.cs`

Every rule from the spec is tested at its boundaries: extreme min, min-1, min, min+1, mid, max-1, max, max+1, extreme max, and an invalid type where the field is typed in as text. Valid values used for the other fields: Title "Never pay before viewing", Description "Do not pay a deposit before you have seen the flat.", Category "Payment", Priority "3", DateAdded today.

**Result: 53 of 53 SafetyTip unit tests passed** (`dotnet test --filter SafetyTipTests`). All 10 stored procedure tests, 10 API tests and 9 page tests also passed.


## 1. Unit tests: SafetyTip class and Valid()

| ID | Field | Boundary | Input | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- | --- | --- |
| T01 | Class | instance | `new SafetyTip()` | Object is created | Object is created | Pass |
| T02 | TipId | property | 5 | Reads back 5 | Reads back 5 | Pass |
| T03 | Title | property | "Test title" | Reads back the same | Reads back the same | Pass |
| T04 | Description | property | test text | Reads back the same | Reads back the same | Pass |
| T05 | Category | property | "Payment" | Reads back the same | Reads back the same | Pass |
| T06 | Priority | property | 3 | Reads back 3 | Reads back 3 | Pass |
| T07 | IsPublished | property | true | Reads back true | Reads back true | Pass |
| T08 | IsPublished | default | new tip | false (hidden) | false | Pass |
| T09 | DateAdded | property | a date | Reads back the same | Reads back the same | Pass |
| T10 | All fields | all valid | valid values above | Valid (no error) | Valid (no error) | Pass |
| T11 | Title | min-1 (blank) | 0 characters | Error message | The title may not be blank. | Pass |
| T12 | Title | min | 1 character | Valid (no error) | Valid (no error) | Pass |
| T13 | Title | min+1 | 2 characters | Valid (no error) | Valid (no error) | Pass |
| T14 | Title | mid | 50 characters | Valid (no error) | Valid (no error) | Pass |
| T15 | Title | max-1 | 99 characters | Valid (no error) | Valid (no error) | Pass |
| T16 | Title | max | 100 characters | Valid (no error) | Valid (no error) | Pass |
| T17 | Title | max+1 | 101 characters | Error message | The title must be 100 characters or less. | Pass |
| T18 | Title | extreme max | 1000 characters | Error message | The title must be 100 characters or less. | Pass |
| T19 | Description | extreme min (blank) | 0 characters | Error message | The description must be at least 10 characters. | Pass |
| T20 | Description | min-1 | 9 characters | Error message | The description must be at least 10 characters. | Pass |
| T21 | Description | min | 10 characters | Valid (no error) | Valid (no error) | Pass |
| T22 | Description | min+1 | 11 characters | Valid (no error) | Valid (no error) | Pass |
| T23 | Description | mid | 255 characters | Valid (no error) | Valid (no error) | Pass |
| T24 | Description | max-1 | 499 characters | Valid (no error) | Valid (no error) | Pass |
| T25 | Description | max | 500 characters | Valid (no error) | Valid (no error) | Pass |
| T26 | Description | max+1 | 501 characters | Error message | The description must be 500 characters or less. | Pass |
| T27 | Description | extreme max | 5000 characters | Error message | The description must be 500 characters or less. | Pass |
| T28 | Category | allowed value | "Payment" | Valid (no error) | Valid (no error) | Pass |
| T29 | Category | allowed value | "Contact" | Valid (no error) | Valid (no error) | Pass |
| T30 | Category | allowed value | "Price" | Valid (no error) | Valid (no error) | Pass |
| T31 | Category | allowed value | "Contract" | Valid (no error) | Valid (no error) | Pass |
| T32 | Category | allowed value | "Viewing" | Valid (no error) | Valid (no error) | Pass |
| T33 | Category | blank | "" | Error message | The category must be one of: Payment, Contact, Price, Contract, Viewing. | Pass |
| T34 | Category | not in the list | "Deposit" | Error message | The category must be one of: Payment, Contact, Price, Contract, Viewing. | Pass |
| T35 | Category | wrong capital letter | "payment" | Error message | The category must be one of: Payment, Contact, Price, Contract, Viewing. | Pass |
| T36 | Priority | extreme min | "-1000" | Error message | The priority must be between 1 and 5. | Pass |
| T37 | Priority | min-1 | "0" | Error message | The priority must be between 1 and 5. | Pass |
| T38 | Priority | min | "1" | Valid (no error) | Valid (no error) | Pass |
| T39 | Priority | min+1 | "2" | Valid (no error) | Valid (no error) | Pass |
| T40 | Priority | mid | "3" | Valid (no error) | Valid (no error) | Pass |
| T41 | Priority | max-1 | "4" | Valid (no error) | Valid (no error) | Pass |
| T42 | Priority | max | "5" | Valid (no error) | Valid (no error) | Pass |
| T43 | Priority | max+1 | "6" | Error message | The priority must be between 1 and 5. | Pass |
| T44 | Priority | extreme max | "1000" | Error message | The priority must be between 1 and 5. | Pass |
| T45 | Priority | invalid type | "abc" | Error message | The priority must be a whole number. | Pass |
| T46 | Priority | not a whole number | "2.5" | Error message | The priority must be a whole number. | Pass |
| T47 | DateAdded | extreme past | 100 years ago | Valid (no error) | Valid (no error) | Pass |
| T48 | DateAdded | max-1 | yesterday | Valid (no error) | Valid (no error) | Pass |
| T49 | DateAdded | max | today | Valid (no error) | Valid (no error) | Pass |
| T50 | DateAdded | max+1 | tomorrow | Error message | The date added cannot be in the future. | Pass |
| T51 | DateAdded | extreme future | 100 years ahead | Error message | The date added cannot be in the future. | Pass |
| T52 | DateAdded | invalid type | "this is not a date" | Error message | The date added is not a valid date. | Pass |
| T53 | DateAdded | blank | "" | Error message | The date added is not a valid date. | Pass |

## 2. Stored procedure tests (SQL, VS Code)

| ID | Procedure | Input | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- | --- |
| S01 | Table + test data | run `tblSafetyTip.sql` | 10 rows affected | 10 rows affected | Pass |
| S02 | `_SelectAll` | none | 10 rows, priority 5 first | 10 rows, priority 5 first | Pass |
| S03 | `_FilterByTipId` | 3 | 1 row (TipId 3) | 1 row (TipId 3) | Pass |
| S04 | `_FilterByTipId` | 999 | 0 rows (not found) | 0 rows | Pass |
| S05 | `_Filter` | Category = Payment | 2 rows | 2 rows | Pass |
| S06 | `_Filter` | IsPublished = 0 | 3 rows | 3 rows | Pass |
| S07 | `_Filter` | Contact + published | 2 rows | 2 rows | Pass |
| S08 | `_Insert` | test tip | new TipId 11 | TipId 11 | Pass |
| S09 | `_Update` | TipId 11, new description | description changed | "Updated text." | Pass |
| S10 | `_Delete` | TipId 11 | row removed, find gives 0 rows | 0 rows | Pass |

## 3. API tests (Swagger, http://localhost:5024/swagger)

| ID | Request | Input | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- | --- |
| A01 | GET /api/safetytips | none | 200, all tips | 200, all tips | Pass |
| A02 | GET /api/safetytips | isPublished = false | 200, only hidden tips | 200, only hidden tips | Pass |
| A03 | GET /api/safetytips/{id} | 3 | 200, tip 3 | 200, "Check the landlord's identity" | Pass |
| A04 | GET /api/safetytips/{id} | 999 | 404 | 404, No safety tip with id 999. | Pass |
| A05 | POST /api/safetytips | valid tip | 201, new tip | 201, TipId 13 | Pass |
| A06 | POST /api/safetytips | priority "9" | 400 + Valid() message | 400, The priority must be between 1 and 5. | Pass |
| A07 | PUT /api/safetytips/{id} | id that does not exist | 404 | 404, No safety tip with id 11. | Pass |
| A08 | PUT /api/safetytips/{id} | 13, edited tip | 204 | 204 | Pass |
| A09 | DELETE /api/safetytips/{id} | 13 | 204 | 204 | Pass |
| A10 | GET /api/safetytips/{id} | 13 after delete | 404 | 404, No safety tip with id 13. | Pass |

## 4. Page tests (http://localhost:5173/safetytips)

| ID | Action | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- |
| P01 | Open Safety tips from the menu | Table of tips, priority 5 first | Table of tips, priority 5 first | Pass |
| P02 | Category = Payment | Only Payment tips | Only Payment tips (tips 1 and 2) | Pass |
| P03 | Published = Hidden | Only unpublished tips | Only unpublished tips (tips 6, 9 and 10) | Pass |
| P04 | Find by id 3 | Only tip 3 | Only tip 3 | Pass |
| P05 | Find by id 999 | "No safety tip with id 999." | "No safety tip with id 999." | Pass |
| P06 | Add a valid tip | "Tip added." and tip in table | "Tip added." and Test tip in the table | Pass |
| P07 | Add a tip with description "abc" | Red error: at least 10 characters | Red: "The description must be at least 10 characters." Nothing added | Pass |
| P08 | Edit a tip, change the title | "Tip updated." and new title | "Tip updated." and title Test tip edited | Pass |
| P09 | Delete a tip, press OK | "Tip deleted." and tip gone | "Tip deleted." and the tip is gone | Pass |
