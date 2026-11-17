import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { deleteRiskCheck, formatDate, getRiskCheck } from '../../api/riskChecks.js'
import './riskchecks.css'

// DELETE a risk check after confirmation (RSC-27)
export default function RiskCheckDelete() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [check, setCheck] = useState(null)
  const [error, setError] = useState('')
  const [deleting, setDeleting] = useState(false)

  useEffect(() => {
    getRiskCheck(id).then(setCheck).catch((e) => setError(e.message))
  }, [id])

  async function confirmDelete() {
    setDeleting(true)
    try {
      await deleteRiskCheck(id)
      navigate('/riskchecks', { state: { message: `Risk check ${id} deleted.` } })
    } catch (e) {
      setError(e.message)
      setDeleting(false)
    }
  }

  return (
    <>
      <h1>Delete risk check {id}</h1>
      {error && <div className="message error">{error}</div>}
      {check && (
        <div className="card">
          <p>Are you sure you want to delete this risk check? This cannot be undone.</p>
          <p>
            <strong>{check.listingAddress ?? `Listing ${check.listingId}`}</strong><br />
            Score {check.riskScore} ({check.riskLevel}), checked {formatDate(check.checkedDate)}
          </p>
          <div className="report-actions">
            <button className="btn danger" onClick={confirmDelete} disabled={deleting}>
              {deleting ? 'Deleting…' : 'Yes, delete'}
            </button>
            <Link className="btn secondary" to="/riskchecks">Cancel</Link>
          </div>
        </div>
      )}
    </>
  )
}
