
import { useEffect, useState } from 'react'
import PriceBenchmarkForm from './PriceBenchmarkForm'
import {
  getPriceBenchmarkCities,
  getAllPriceBenchmarks,
  findPriceBenchmark,
  filterPriceBenchmarks,
  deletePriceBenchmark,
} from './pricebenchmarksApi'
import './pricebenchmarks.css'

// Price Benchmarks page (owner: Shrayana).
// RSC-36: Add, Edit, Delete, List, Find and Filter.

const noFilters = {
  city: '',
  isActive: '',
}

export default function PriceBenchmarksPage() {
  const [benchmarks, setBenchmarks] = useState([])
  const [cities, setCities] = useState([])
  const [filters, setFilters] = useState(noFilters)
  const [findId, setFindId] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [formFor, setFormFor] = useState(null)

  // Runs an API action and displays errors.
  async function run(action) {
    setError('')
    setMessage('')

    try {
      await action()
    } catch (err) {
      setError(err.message)
    }
  }

  const showAll = () =>
    run(async () => {
      setBenchmarks(await getAllPriceBenchmarks())
    })

  // Load cities and benchmarks when the page opens.
  useEffect(() => {
    Promise.all([
      getPriceBenchmarkCities(),
      getAllPriceBenchmarks(),
    ])
      .then(([c, b]) => {
        setCities(c)
        setBenchmarks(b)
      })
      .catch((err) => setError(err.message))
  }, [])

  // Find one benchmark using its ID.
  function find(e) {
    e.preventDefault()

    const id = Number(findId)

    if (!Number.isInteger(id) || id < 1) {
      setError('Please enter a valid benchmark ID.')
      return
    }

    run(async () => {
      const result = await findPriceBenchmark(id)
      setBenchmarks([result])
    })
  }

  // Filter by city and active status.
  function applyFilters(e) {
    e.preventDefault()

    run(async () => {
      const result = await filterPriceBenchmarks(filters)
      setBenchmarks(result)

      if (result.length === 0) {
        setMessage('No price benchmarks match your filters.')
      }
    })
  }

  function clearFilters() {
    setFilters(noFilters)
    setFindId('')
    showAll()
  }

  // Delete an existing benchmark.
  function remove(benchmark) {
    if (
      !window.confirm(
        `Are you sure you want to delete benchmark ${benchmark.benchmarkId} for ${benchmark.city}?`
      )
    ) {
      return
    }

    run(async () => {
      await deletePriceBenchmark(benchmark.benchmarkId)
      setBenchmarks(await getAllPriceBenchmarks())
      setMessage('Price benchmark deleted.')
    })
  }

  // Called after the Add or Edit form saves successfully.
  function saved(benchmark, text) {
    setFormFor(null)

    run(async () => {
      setBenchmarks(await getAllPriceBenchmarks())
      setMessage(text)
    })
  }

  const setFilter = (field) => (e) => {
    setFilters((previous) => ({
      ...previous,
      [field]: e.target.value,
    }))
  }

  // Show Add/Edit form when requested.
  if (formFor) {
    return (
      <PriceBenchmarkForm
        benchmark={formFor === 'new' ? null : formFor}
        cities={cities}
        onSaved={saved}
        onCancel={() => setFormFor(null)}
      />
    )
  }

  return (
    <div className="pricebenchmarks">
      <div className="pricebenchmarks-header">
        <h1>Price Benchmarks</h1>

        <button onClick={() => setFormFor('new')}>
          Add a benchmark
        </button>
      </div>

      {/* Find by benchmark ID */}
      <form className="toolbar" onSubmit={find}>
        <input
          type="number"
          min="1"
          placeholder="Benchmark ID"
          value={findId}
          onChange={(e) => setFindId(e.target.value)}
        />

        <button type="submit">Find</button>
      </form>

      {/* Filter by city and active status */}
      <form className="toolbar" onSubmit={applyFilters}>
        <select
          value={filters.city}
          onChange={setFilter('city')}
        >
          <option value="">All cities</option>

          {cities.map((city) => (
            <option key={city} value={city}>
              {city}
            </option>
          ))}
        </select>

        <select
          value={filters.isActive}
          onChange={setFilter('isActive')}
        >
          <option value="">All statuses</option>
          <option value="true">Active</option>
          <option value="false">Inactive</option>
        </select>

        <button type="submit">Filter</button>
        <button type="button" onClick={clearFilters}>
          Clear
        </button>
      </form>

      {error && <p className="error">{error}</p>}
      {message && <p className="message">{message}</p>}

      {benchmarks.length === 0 && !error && !message ? (
        <p>No price benchmarks yet. Add a benchmark to get started.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>City</th>
              <th>Private rent (kr/m²)</th>
              <th>Social rent (kr/m²)</th>
              <th>Status</th>
              <th>Data date</th>
              <th>Source reference</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {benchmarks.map((b) => (
              <tr key={b.benchmarkId}>
                <td>{b.benchmarkId}</td>
                <td>{b.city}</td>

                <td>
                  {b.privateRentPerM2 == null
                    ? '—'
                    : Number(b.privateRentPerM2).toLocaleString(
                        'da-DK',
                        { minimumFractionDigits: 2 }
                      )}
                </td>

                <td>
                  {b.socialRentPerM2 == null
                    ? '—'
                    : Number(b.socialRentPerM2).toLocaleString(
                        'da-DK',
                        { minimumFractionDigits: 2 }
                      )}
                </td>

                <td>{b.isActive ? 'Active' : 'Inactive'}</td>

                <td>{b.dataDate?.slice(0, 10)}</td>

                <td>{b.sourceReference}</td>

                <td className="actions">
                  <button onClick={() => setFormFor(b)}>
                    Edit
                  </button>

                  <button onClick={() => remove(b)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
