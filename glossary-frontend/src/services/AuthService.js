import axios from "axios";
import { handleApiErrors } from "./errorHandler";
const BASE_URL = "https://localhost:7163/api/identity";

export const login = async (username, password) => {
	try {
		const response = await axios.post(`${BASE_URL}/login`, {
			username,
			password,
		});
		const token = response.data.token;
		localStorage.setItem("token", token);

		return { token, errors: null };
	} catch (error) {
		const errors = handleApiErrors(error);
		return { token: null, errors };
	}
};

export const register = async (username, password, email, confirmPassword) => {
	try {
		const response = await axios.post(`${BASE_URL}/register`, {
			username,
			password,
			confirmPassword,
			email,
		});
		return { message: response.data.message, errors: null };
	} catch (error) {
		const errors = handleApiErrors(error);
		return { message: null, errors };
	}
};
