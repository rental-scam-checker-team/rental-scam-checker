
import { NavLink, Navigate, Route, Routes } from 'react-router-dom'

import RiskCheckList from './pages/riskchecks/RiskCheckList.jsx'
import RiskCheckForm from './pages/riskchecks/RiskCheckForm.jsx'
import RiskCheckDelete from './pages/riskchecks/RiskCheckDelete.jsx'
import RiskReport from './pages/riskchecks/RiskReport.jsx'
import ListingsPage from './listings/ListingsPage.jsx'
import PriceBenchmarksPage from './pricebenchmarks/PriceBenchmarksPage.jsx'
import SafetyTipsPage from './safetytips/SafetyTipsPage.jsx'

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <span className="brand">Rental Scam Checker</span>

        <nav>
          <NavLink to="/listings">Listings</NavLink>
          <NavLink to="/riskchecks">Risk checks</NavLink>
          <NavLink to="/pricebenchmarks">Price Benchmarks</NavLink>
          <NavLink to="/safetytips">Safety tips</NavLink>
        </nav>
      </header>

      <main className="content">
        <Routes>
          <Route path="/" element={<Navigate to="/riskchecks" replace />} />

          {/* Listings */}
          <Route path="/listings" element={<ListingsPage />} />

          {/* Risk checks */}
          <Route path="/riskchecks" element={<RiskCheckList />} />
          <Route path="/riskchecks/new" element={<RiskCheckForm />} />
          <Route path="/riskchecks/:id" element={<RiskReport />} />
          <Route path="/riskchecks/:id/edit" element={<RiskCheckForm />} />
          <Route path="/riskchecks/:id/delete" element={<RiskCheckDelete />} />

          {/* Price Benchmarks */}
          <Route path="/pricebenchmarks" element={<PriceBenchmarksPage />} />

          {/* Safety tips */}
          <Route path="/safetytips" element={<SafetyTipsPage />} />

          <Route path="*" element={<p>Page not found.</p>} />
        </Routes>
      </main>
    </div>
  )
}
