import { NavLink, Navigate, Route, Routes } from 'react-router-dom'
import RiskCheckList from './pages/riskchecks/RiskCheckList.jsx'
import RiskCheckForm from './pages/riskchecks/RiskCheckForm.jsx'
import RiskCheckDelete from './pages/riskchecks/RiskCheckDelete.jsx'
import RiskReport from './pages/riskchecks/RiskReport.jsx'
import ListingsPage from './listings/ListingsPage.jsx'
import PriceBenchmarksPage from './pricebenchmarks/PriceBenchmarksPage.jsx'
// App shell: top bar + one route per page.
// Each team member adds their own nav link and routes here (Listings, Price benchmarks, Safety tips).
export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <span className="brand">Rental Scam Checker</span>
        <nav>
          <NavLink to="/listings">Listings</NavLink>
          <NavLink to="/riskchecks">Risk checks</NavLink>
          <NavLink to="/pricebenchmarks">Price Benchmarks</NavLink>
        </nav>
      </header>

      <main className="content">
        <Routes>
          <Route path="/" element={<Navigate to="/riskchecks" replace />} />

          {/* Listings (Aman Paudel) */}
          <Route path="/listings" element={<ListingsPage />} />

          {/* Risk checks (Guru Prasad Kandel) */}
          <Route path="/riskchecks" element={<RiskCheckList />} />
          <Route path="/riskchecks/new" element={<RiskCheckForm />} />
          <Route path="/riskchecks/:id" element={<RiskReport />} />
          <Route path="/riskchecks/:id/edit" element={<RiskCheckForm />} />
          <Route path="/riskchecks/:id/delete" element={<RiskCheckDelete />} />
          <Route path="/pricebenchmarks" element={<PriceBenchmarksPage />} />
 {/* Pricebenchmarks (Shrayana Kalika) */}
 <Route
  path="/pricebenchmarks"
  element={<PriceBenchmarksPage />}
/>
          <Route path="*" element={<p>Page not found.</p>} />
        </Routes>
      </main>
    </div>
  )
}
