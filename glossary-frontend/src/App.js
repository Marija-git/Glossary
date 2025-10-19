import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import AuthPage from "./pages/AuthPage";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function App() {
	return (
		<Router>
			<Routes>
				<Route
					path='/login'
					element={<AuthPage />}
				/>
				<Route
					path='/'
					element={<HomePage />}
				/>
			</Routes>
			<ToastContainer
				position='top-right'
				autoClose={3000}
			/>
		</Router>
	);
}

export default App;
