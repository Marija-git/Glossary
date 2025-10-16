// src/pages/AuthPage.jsx
import { useState } from "react";
import { Container, Card, Form, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const AuthPage = () => {
	const [isLogin, setIsLogin] = useState(true);
	const [form, setForm] = useState({ email: "", username: "", password: "" });
	const navigate = useNavigate();

	const handleChange = (e) => {
		setForm({ ...form, [e.target.name]: e.target.value });
	};

	const handleSubmit = (e) => {
		e.preventDefault();
		if (isLogin) {
			console.log("Login:", form.username, form.password);
			// login
		} else {
			console.log("Register:", form.email, form.username, form.password);
			// register
		}
		//  redirect
		navigate("/homepage");
	};

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

					<Button
						variant={isLogin ? "primary" : "success"}
						type='submit'
						className='w-100 mb-2'>
						{isLogin ? "Login" : "Register"}
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
