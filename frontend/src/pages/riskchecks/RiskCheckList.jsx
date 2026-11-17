import { useEffect, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { formatDate, listRiskChecks } from '../../api/riskChecks.js'
import './riskchecks.css'

const EMPTY_FILTERS = { minScore: '', maxScore: '', from: '', to: '', notified: '' }

// LIST, FIND and FILTER risk checks (RSC-28, RSC-29, RSC-30)
export default function RiskCheckList() {
  const location = useLocation()
  const [checks, setChecks] = useState([])
  const [filters, setFilters] = useState(EMPTY_FILTERS)
  const [findId, setFindId] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  async function load(activeFilters) {
    setLoading(true)
    setError('')
    try {
      setChecks(await listRiskChecks(activeFilters))
    } catch (e) {
      setError(e.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    listRiskChecks()
      .then(setChecks)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [])

  function change(e) {
    setFilters({ ...filters, [e.target.name]: e.target.value })
  }

  function applyFilters(e) {
    e.preventDefault()
    if (filters.minScore !== '' && filters.maxScore !== '' && Number(filters.minScore) > Number(filters.maxScore)) {
      setError('Minimum score cannot be higher than maximum score.')
      return
    }
    load(filters)
  }

  function clearFilters() {
    setFilters(EMPTY_FILTERS)
    load(EMPTY_FILTERS)
  }

  return (
    <>
      <div className="rc-header">
        <h1>Risk checks</h1>
        <Link className="btn" to="/riskchecks/new">Add risk check</Link>
      </div>

      {location.state?.message && <div className="message success">{location.state.message}</div>}
      {error && <div className="message error">{error}</div>}

      <form className="card rc-filters" onSubmit={applyFilters}>
        <div>
          <label htmlFor="minScore">Min score</label>
          <input id="minScore" name="minScore" type="number" min="0" max="100" value={filters.minScore} onChange={change} />
        </div>
        <div>
          <label htmlFor="maxScore">Max score</label>
          <input id="maxScore" name="maxScore" type="number" min="0" max="100" value={filters.maxScore} onChange={change} />
        </div>
        <div>
          <label htmlFor="from">From date</label>
          <input id="from" name="from" type="date" value={filters.from} onChange={change} />
        </div>
        <div>
          <label htmlFor="to">To date</label>
          <input id="to" name="to" type="date" value={filters.to} onChange={change} />
        </div>
        <div>
          <label htmlFor="notified">Report read?</label>
          <select id="notified" name="notified" value={filters.notified} onChange={change}>
            <option value="">Any</option>
            <option value="true">Yes</option>
            <option value="false">No</option>
          </select>
        </div>
        <div className="actions">
          <button className="btn" type="submit">Filter</button>
          <button className="btn secondary" type="button" onClick={clearFilters}>Clear</button>
        </div>
      </form>

      <form className="rc-header" onSubmit={(e) => e.preventDefault()}>
        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'end' }}>
          <div>
            <label htmlFor="findId">Find by ID</label>
            <input id="findId" type="number" min="1" value={findId} onChange={(e) => setFindId(e.target.value)} />
          </div>
          <Link className="btn secondary" to={findId ? `/riskchecks/${findId}` : '#'}>Open</Link>
        </div>
      </form>

      {loading ? (
        <p>Loading…</p>
      ) : checks.length === 0 ? (
        <div className="card">No risk checks match. Try clearing the filters.</div>
      ) : (
        <table className="rc-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Listing</th>
              <th>Score</th>
              <th>Level</th>
              <th>Checked</th>
              <th>Read</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {checks.map((c) => (
              <tr key={c.riskCheckId} className={c.userNotified ? '' : 'unread'}>
                <td className="num">{c.riskCheckId}</td>
                <td>{c.listingAddress ?? `Listing ${c.listingId}`}</td>
                <td className="num">{c.riskScore}</td>
                <td><span className={`level ${c.riskLevel}`}>{c.riskLevel}</span></td>
                <td>{formatDate(c.checkedDate)}</td>
                <td>{c.userNotified ? 'Yes' : 'No'}</td>
                <td className="row-actions">
                  <Link to={`/riskchecks/${c.riskCheckId}`}>Report</Link>
                  <Link to={`/riskchecks/${c.riskCheckId}/edit`}>Edit</Link>
                  <Link to={`/riskchecks/${c.riskCheckId}/delete`}>Delete</Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  )
}
