import { useEffect, useState } from 'react'
import ListingForm from './ListingForm'
import {
  getCities, getAllListings, findListing, filterListings, deleteListing,
} from './listingsApi'
import './listings.css'

// Listings page (owner: Aman Paudel).
// List (RSC-21), Find (RSC-22), Filter (RSC-23), Delete (RSC-20),
// and it opens the form for Add (RSC-18) and Edit (RSC-19).
const noFilters = { area: '', minRent: '', maxRent: '', furnished: '' }

export default function ListingsPage() {
  const [listings, setListings] = useState([])
  const [cities, setCities] = useState([])
  const [filters, setFilters] = useState(noFilters)
  const [findId, setFindId] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [formFor, setFormFor] = useState(null) // null = no form, 'new' = add, a listing = edit

  // Runs an API call and shows its error message if it fails
  async function run(action) {
    setError('')
    setMessage('')
    try {
      await action()
    } catch (err) {
      setError(err.message)
    }
  }

  const showAll = () => run(async () => setListings(await getAllListings()))

  // Load the cities and all listings once, when the page opens
  useEffect(() => {
    Promise.all([getCities(), getAllListings()])
      .then(([c, l]) => {
        setCities(c)
        setListings(l)
      })
      .catch((err) => setError(err.message))
  }, [])

  function find(e) {
    e.preventDefault()
    const id = Number(findId)
    if (!Number.isInteger(id) || id < 1) {
      setError('Please enter a valid listing ID.')
      return
    }
    run(async () => setListings([await findListing(id)]))
  }

  function applyFilters(e) {
    e.preventDefault()
    run(async () => {
      const result = await filterListings(filters)
      setListings(result)
      if (result.length === 0) setMessage('No listings match your filters.')
    })
  }

  function clearFilters() {
    setFilters(noFilters)
    setFindId('')
    showAll()
  }

  function remove(listing) {
    if (!window.confirm(`Are you sure you want to delete this listing?\n${listing.address}`)) return
    run(async () => {
      await deleteListing(listing.listingId)
      setListings(await getAllListings())
      setMessage('Listing deleted.')
    })
  }

  function saved(listing, text) {
    setFormFor(null)
    run(async () => {
      setListings(await getAllListings())
      setMessage(text)
    })
  }

  if (formFor) {
    return (
      <ListingForm
        listing={formFor === 'new' ? null : formFor}
        cities={cities}
        onSaved={saved}
        onCancel={() => setFormFor(null)}
      />
    )
  }

  const setFilter = (field) => (e) => setFilters({ ...filters, [field]: e.target.value })

  return (
    <div className="listings">
      <div className="listings-header">
        <h1>Listings</h1>
        <button onClick={() => setFormFor('new')}>Add a listing</button>
      </div>

      <form className="toolbar" onSubmit={find}>
        <input type="number" min="1" placeholder="Listing ID" value={findId}
               onChange={(e) => setFindId(e.target.value)} />
        <button type="submit">Find</button>
      </form>

      <form className="toolbar" onSubmit={applyFilters}>
        <select value={filters.area} onChange={setFilter('area')}>
          <option value="">All cities</option>
          {cities.map((c) => <option key={c} value={c}>{c}</option>)}
        </select>
        <input type="number" min="0" placeholder="Min rent" value={filters.minRent} onChange={setFilter('minRent')} />
        <input type="number" min="0" placeholder="Max rent" value={filters.maxRent} onChange={setFilter('maxRent')} />
        <select value={filters.furnished} onChange={setFilter('furnished')}>
          <option value="">Furnished or not</option>
          <option value="true">Furnished</option>
          <option value="false">Not furnished</option>
        </select>
        <button type="submit">Filter</button>
        <button type="button" onClick={clearFilters}>Clear</button>
      </form>

      {error && <p className="error">{error}</p>}
      {message && <p className="message">{message}</p>}

      {listings.length === 0 && !error && !message ? (
        <p>No listings yet. Add a listing to get started.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>ID</th><th>Address</th><th>City</th><th>Rent (kr)</th>
              <th>Size (m²)</th><th>Posted</th><th>Furnished</th><th></th>
            </tr>
          </thead>
          <tbody>
            {listings.map((l) => (
              <tr key={l.listingId}>
                <td>{l.listingId}</td>
                <td>{l.address}</td>
                <td>{l.area}</td>
                <td>{l.monthlyRent.toLocaleString('da-DK')}</td>
                <td>{l.sizeM2}</td>
                <td>{l.postedDate.slice(0, 10)}</td>
                <td>{l.furnished ? 'Yes' : 'No'}</td>
                <td className="actions">
                  {/* Edit and Delete are admin functions: hide them for normal users when the shared login is ready */}
                  <button onClick={() => setFormFor(l)}>Edit</button>
                  <button onClick={() => remove(l)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
