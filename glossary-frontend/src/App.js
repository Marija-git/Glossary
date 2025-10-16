import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import GlossaryNavbar from "./components/GlossaryNavbar";
import AuthPage from "./pages/AuthPage";

function App() {
	return (
		<Router>
			<Routes>
				<Route
					path='/login'
					element={<AuthPage />}
				/>
				<Route
					path='/homepage'
					element={<HomePage />}
				/>
			</Routes>
		</Router>
	);
}

export default App;
