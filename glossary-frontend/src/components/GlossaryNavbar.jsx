import { Navbar, Container, Nav } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.css";
import { useSelector, useDispatch } from "react-redux";
import { logout } from "../store/AuthSlice";
import { resetGlossary } from "../store/GlossaryTermSlice";

const GlossaryNavbar = () => {
	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);
	const dispatch = useDispatch();
	const navigate = useNavigate();
	const handleLogout = () => {
		dispatch(logout());
		dispatch(resetGlossary());
		navigate("/login");
	};
	return (
		<Navbar
			bg='primary'
			variant='dark'
			expand='lg'>
			<Container>
				<Navbar.Brand
					as={Link}
					to='/homepage'>
					Glossary App
				</Navbar.Brand>

				<Nav className='ms-auto'>
					{isLoggedIn ? (
						<Nav.Link onClick={handleLogout}>Logout</Nav.Link>
					) : (
						<Nav.Link
							as={Link}
							to='/login'>
							Login / Sign In
						</Nav.Link>
					)}
				</Nav>
			</Container>
		</Navbar>
	);
};

export default GlossaryNavbar;
