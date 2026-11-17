import { useState } from 'react'
import { addListing, updateListing } from './listingsApi'

// Add a listing (RSC-18) and Edit a listing (RSC-19).
// If `listing` is given, the form is in Edit mode and starts with its values.
export default function ListingForm({ listing, cities, onSaved, onCancel }) {
  const editing = Boolean(listing)
  // today's date as yyyy-mm-dd in local time (the posted date cannot be after this)
  const [today] = useState(() => new Date().toLocaleDateString('sv-SE'))

  const [form, setForm] = useState({
    address: listing?.address ?? '',
    area: listing?.area ?? '',
    monthlyRent: listing ? String(listing.monthlyRent) : '',
    sizeM2: listing ? String(listing.sizeM2) : '',
    postedDate: listing ? listing.postedDate.slice(0, 10) : today,
    furnished: listing?.furnished ?? false,
  })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  const change = (field) => (e) => {
    const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm({ ...form, [field]: value })
  }

  async function save(e) {
    e.preventDefault()
    setError('')
    setSaving(true)
    // Everything is sent as text. The API runs Listing.Valid() and sends back
    // one message per wrong field if something is not right.
    const data = { ...form, furnished: String(form.furnished) }
    try {
      const saved = editing ? await updateListing(listing.listingId, data) : await addListing(data)
      onSaved(saved, editing ? 'Listing updated.' : 'Listing saved.')
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  return (
    <form className="listing-form" onSubmit={save}>
      <h2>{editing ? `Edit listing ${listing.listingId}` : 'Add a listing'}</h2>

      <label>
        Address
        <input value={form.address} onChange={change('address')} maxLength={100}
               placeholder="Nørrebrogade 45, 2. th." />
      </label>

      <label>
        City
        <select value={form.area} onChange={change('area')}>
          <option value="">Choose a city</option>
          {cities.map((c) => <option key={c} value={c}>{c}</option>)}
        </select>
      </label>

      <label>
        Monthly rent (kr)
        <input type="number" step="0.01" min="0" value={form.monthlyRent} onChange={change('monthlyRent')} />
      </label>

      <label>
        Size (m²)
        <input type="number" step="1" value={form.sizeM2} onChange={change('sizeM2')} />
      </label>

      <label>
        Date the ad was posted
        <input type="date" max={today} value={form.postedDate} onChange={change('postedDate')} />
      </label>

      <label className="checkbox">
        <input type="checkbox" checked={form.furnished} onChange={change('furnished')} />
        Furnished
      </label>

      {error && <p className="error">{error}</p>}

      <div className="buttons">
        <button type="submit" disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
        <button type="button" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  )
}
