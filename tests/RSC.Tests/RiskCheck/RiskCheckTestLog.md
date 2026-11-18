# Test log: Risk checks (tblRiskCheck)

Owner: Guru Prasad Kandel · Jira: RSC-35 (tests for RSC-25 to RSC-30, RSC-34) · Run date: 2026-11-18

Test files: `tests/RSC.Tests/RiskCheck/RiskCheckValidTests.cs` (validation and risk level), `tests/RSC.Tests/RiskCheck/RiskCheckDatabaseTests.cs` (data layer against SQL Server Express). API tested by hand in Swagger.

**Result: 51 of 51 Risk check unit tests passed.** Run together with the rest of the team's tests on main (Listings, Safety tips): 168 of 168 passed, 0 failed (`screenshots/00_dotnet_test_168.png`). All 8 API tests gave the expected result.

Valid values used for the other fields: ListingId "1", RiskScore "50", Flags "Rent 55% below area average", ReviewNote "Checked by admin", CheckedDate today.

## 1. Validation tests – RiskCheck.Valid()

| ID | Field | Boundary | Input | Expected result | Actual result | Pass/Fail |
| --- | --- | --- | --- | --- | --- | --- |
| V01 | ListingId | valid data (all fields) | defaults | Valid (no error) | Valid (no error) | Pass |
| V02 | ListingId | min | 1 | Valid (no error) | Valid (no error) | Pass |
| V03 | ListingId | min-1 | 0 | Error message | Error message | Pass |
| V04 | ListingId | extreme min | -1 | Error message | Error message | Pass |
| V05 | ListingId | blank | "" | Error message | Error message | Pass |
| V06 | ListingId | wrong type | abc | Error message | Error message | Pass |
| V07 | RiskScore | min-1 | -1 | Error message | Error message | Pass |
| V08 | RiskScore | min | 0 | Valid (no error) | Valid (no error) | Pass |
| V09 | RiskScore | min+1 | 1 | Valid (no error) | Valid (no error) | Pass |
| V10 | RiskScore | mid | 50 | Valid (no error) | Valid (no error) | Pass |
| V11 | RiskScore | max-1 | 99 | Valid (no error) | Valid (no error) | Pass |
| V12 | RiskScore | max | 100 | Valid (no error) | Valid (no error) | Pass |
| V13 | RiskScore | max+1 | 101 | Error message | Error message | Pass |
| V14 | RiskScore | extreme max | 100000 | Error message | Error message | Pass |
| V15 | RiskScore | wrong type | high | Error message | Error message | Pass |
| V16 | RiskScore | blank | "" | Error message | Error message | Pass |
| V17 | Flags | min-1 (blank) | 0 characters | Error message | Error message | Pass |
| V18 | Flags | min | 1 character | Valid (no error) | Valid (no error) | Pass |
| V19 | Flags | mid | 250 characters | Valid (no error) | Valid (no error) | Pass |
| V20 | Flags | max-1 | 499 characters | Valid (no error) | Valid (no error) | Pass |
| V21 | Flags | max | 500 characters | Valid (no error) | Valid (no error) | Pass |
| V22 | Flags | max+1 | 501 characters | Error message | Error message | Pass |
| V23 | ReviewNote | optional (null) | null | Valid (no error) | Valid (no error) | Pass |
| V24 | ReviewNote | min (empty) | 0 characters | Valid (no error) | Valid (no error) | Pass |
| V25 | ReviewNote | max-1 | 249 characters | Valid (no error) | Valid (no error) | Pass |
| V26 | ReviewNote | max | 250 characters | Valid (no error) | Valid (no error) | Pass |
| V27 | ReviewNote | max+1 | 251 characters | Error message | Error message | Pass |
| V28 | CheckedDate | max | today | Valid (no error) | Valid (no error) | Pass |
| V29 | CheckedDate | max-1 | yesterday | Valid (no error) | Valid (no error) | Pass |
| V30 | CheckedDate | max+1 | tomorrow | Error message | Error message | Pass |
| V31 | CheckedDate | extreme min | 100 years ago | Valid (no error) | Valid (no error) | Pass |
| V32 | CheckedDate | wrong type | not a date | Error message | Error message | Pass |
| V33 | CheckedDate | impossible date | 2026-02-30 | Error message | Error message | Pass |

## 2. Risk level tests – RiskCheck.RiskLevel

Rule: score 0–39 = Low, 40–69 = Medium, 70–100 = High.

| ID | Input score | Boundary | Expected | Actual | Pass/Fail |
| --- | --- | --- | --- | --- | --- |
| L01 | 0 | min | Low | Low | Pass |
| L02 | 39 | Low max | Low | Low | Pass |
| L03 | 40 | Medium min | Medium | Medium | Pass |
| L04 | 69 | Medium max | Medium | Medium | Pass |
| L05 | 70 | High min | High | High | Pass |
| L06 | 100 | max | High | High | Pass |

## 3. Data layer tests – RiskCheckCollection (database)

| ID | Test | Action | Expected | Actual | Pass/Fail |
| --- | --- | --- | --- | --- | --- |
| D01 | ListReturnsTestData | GetAll() on the test data | Rows returned | Rows returned | Pass |
| D02 | ListIsNewestFirst | GetAll() | Newest CheckedDate first | Newest CheckedDate first | Pass |
| D03 | AddThenFindReturnsSameData | Add() then Find(new id) | Same values read back | Same values read back | Pass |
| D04 | FindMissingIdReturnsFalse | Find(id that does not exist) | false | false | Pass |
| D05 | UpdateChangesData | Update() then Find() | New values saved | New values saved | Pass |
| D06 | UpdateMissingIdReturnsFalse | Update(missing id) | false | false | Pass |
| D07 | DeleteRemovesRecord | Delete() then Find() | Record gone | Record gone | Pass |
| D08 | DeleteMissingIdReturnsFalse | Delete(missing id) | false | false | Pass |
| D09 | FilterByScoreRange | Filter(min 70, max 100) | Only scores 70-100 | Only scores 70-100 | Pass |
| D10 | FilterByNotified | Filter(notified true) | Only UserNotified = true | Only UserNotified = true | Pass |
| D11 | FilterByDateRange | Filter(from, to) | Only dates in range | Only dates in range | Pass |
| D12 | FilterWithNoValuesReturnsAll | Filter() with no values | Same as GetAll() | Same as GetAll() | Pass |

## 4. API tests – Swagger (http://localhost:5024/swagger)

| ID | Request | Input | Expected | Actual | Pass/Fail | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| A01 | GET /api/riskchecks | none | 200 + list of risk checks with address and risk level | 200 OK, list returned | Pass | `screenshots/01_get_all.png` |
| A02 | GET /api/riskchecks/1 | id 1 | 200 + risk check 1 | 200 OK, risk check 1 returned | Pass | `screenshots/02_get_by_id.png` |
| A03 | POST /api/riskchecks | listingId 1, score 85, flags "price too low" | 201 Created + new id, riskLevel High | 201 Created, id 24, riskLevel High | Pass | `screenshots/03_post.png` |
| A04 | PUT /api/riskchecks/24 | score 40, note "updated after review", notified true | 204 No Content (updated) | 204 No Content | Pass | `screenshots/04_put.png` |
| A05 | POST /api/riskchecks | same as A03 | 201 Created + new id | 201 Created, id 25 | Pass | `screenshots/05_post_id25.jpg` |
| A06 | DELETE /api/riskchecks/25 | id 25 | 204 No Content (deleted) | 204 No Content | Pass | `screenshots/06_delete_204.jpg` |
| A07 | DELETE /api/riskchecks/24 | id that no longer exists | 404 + error message | 404, "No risk check with ID 24." | Pass | `screenshots/07_delete_missing_404.png` |
| A08 | POST /api/riskchecks | riskScore 150 (invalid) | 400 + validation message | 400, "Risk score must be between 0 and 100." | Pass | `screenshots/08_post_invalid_400.jpg` |

## Notes

- PUT and DELETE return 204 No Content on success, which is the standard response when nothing needs to be sent back.
- A07 was found by accident: DELETE was run twice on id 24. The second call correctly returned 404, so it was kept as a test.
- The address in the API responses comes from joining tblRiskCheck to tblListing on ListingId.
