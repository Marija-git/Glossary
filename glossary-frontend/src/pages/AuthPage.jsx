import { useState, useEffect } from "react";
import { Container, Card, Form, Button, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { loginUser, registerUser, clearMessages } from "../store/AuthSlice";
import { toast } from "react-toastify";

const AuthPage = () => {
	const [isLogin, setIsLogin] = useState(true);
	const [form, setForm] = useState({
		email: "",
		username: "",
		password: "",
		confirmPassword: "",
	});

	const dispatch = useDispatch();
	const navigate = useNavigate();

	const { loading, error, successMessage, isAuthenticated } = useSelector(
		(state) => state.auth
	);

	const handleChange = (e) => {
		setForm({ ...form, [e.target.name]: e.target.value });
	};

	const handleSubmit = async (e) => {
		e.preventDefault();
		dispatch(clearMessages());
		let result;
		if (isLogin) {
			result = await dispatch(
				loginUser({ username: form.username, password: form.password })
			);
			if (result.type.endsWith("fulfilled")) {
				navigate("/homepage");
			}
		} else {
			result = await dispatch(
				registerUser({
					username: form.username,
					password: form.password,
					email: form.email,
					confirmPassword: form.confirmPassword,
				})
			);
			if (result.type.endsWith("fulfilled")) {
				setIsLogin(true);
				navigate("/login");
			}
		}
	};

	useEffect(() => {
		if (isAuthenticated) {
			navigate("/homepage");
		}
	}, [isAuthenticated, navigate]);

	useEffect(() => {
		dispatch(clearMessages());
	}, [isLogin, dispatch]);

	useEffect(() => {
		if (error) toast.error(error);
		if (successMessage) toast.success(successMessage);
	}, [error, successMessage]);

	useEffect(() => {
		setForm({
			email: "",
			username: "",
			password: "",
			confirmPassword: "",
		});
	}, [isLogin]);

	return (
		<Container
			className='d-flex justify-content-center align-items-center'
			style={{ minHeight: "80vh" }}>
			<Card
				className='p-4 shadow-lg'
				style={{ maxWidth: "450px", width: "100%" }}>
				<h3 className='text-center mb-4'>{isLogin ? "Login" : "Register"}</h3>

				<Form onSubmit={handleSubmit}>
					{!isLogin && (
						<Form.Group
							className='mb-3'
							controlId='formEmail'>
							<Form.Label>Email</Form.Label>
							<Form.Control
								type='email'
								placeholder='Enter email'
								name='email'
								value={form.email}
								onChange={handleChange}
								required
							/>
						</Form.Group>
					)}

					<Form.Group
						className='mb-3'
						controlId='formUsername'>
						<Form.Label>Username</Form.Label>
						<Form.Control
							type='text'
							placeholder='Enter username'
							name='username'
							value={form.username}
							onChange={handleChange}
							required
						/>
					</Form.Group>

					<Form.Group
						className='mb-3'
						controlId='formPassword'>
						<Form.Label>Password</Form.Label>
						<Form.Control
							type='password'
							placeholder='Enter password'
							name='password'
							value={form.password}
							onChange={handleChange}
							required
						/>
					</Form.Group>

					{!isLogin && (
						<Form.Group
							className='mb-3'
							controlId='formConfirmPassword'>
							<Form.Label>Confirm Password</Form.Label>
							<Form.Control
								type='password'
								placeholder='Confirm password'
								name='confirmPassword'
								value={form.confirmPassword}
								onChange={handleChange}
								required
							/>
						</Form.Group>
					)}

					<Button
						variant={isLogin ? "primary" : "success"}
						type='submit'
						className='w-100 mb-2'
						disabled={loading}>
						{loading ? (
							<>
								<Spinner
									size='sm'
									className='me-2'
								/>{" "}
								Loading...
							</>
						) : isLogin ? (
							"Login"
						) : (
							"Register"
						)}
					</Button>
				</Form>

				<Button
					variant='link'
					className='w-100'
					onClick={() => setIsLogin(!isLogin)}>
					{isLogin
						? "Don't have an account? Sign up"
						: "Already have an account? Login"}
				</Button>
			</Card>
		</Container>
	);
};

export default AuthPage;
