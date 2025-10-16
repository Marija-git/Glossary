import { Navbar, Container, Nav } from "react-bootstrap";
import { Link } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.css";

const GlossaryNavbar = ({ isLoggedIn = true }) => {
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
						<Nav.Link
							as={Link}
							to='/login'>
							Logout
						</Nav.Link>
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
