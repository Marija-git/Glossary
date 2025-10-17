import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { login, register } from "../services/AuthService";

export const loginUser = createAsyncThunk(
	"auth/loginUser",
	async ({ username, password }, thunkAPI) => {
		const { token, errors } = await login(username, password);
		if (errors) return thunkAPI.rejectWithValue(errors);

		localStorage.setItem("username", username);

		return { token, username };
	}
);

export const registerUser = createAsyncThunk(
	"auth/registerUser",
	async ({ username, password, email, confirmPassword }, thunkAPI) => {
		const { message, errors } = await register(
			username,
			password,
			email,
			confirmPassword
		);
		if (errors) return thunkAPI.rejectWithValue(errors);
		return { message };
	}
);

const initialState = {
	token: localStorage.getItem("token") || null,
	username: localStorage.getItem("username") || "",
	isAuthenticated: !!localStorage.getItem("token"),
	loading: false,
	error: null,
	successMessage: null,
};

const authSlice = createSlice({
	name: "auth",
	initialState,
	reducers: {
		logout(state) {
			localStorage.removeItem("token");
			localStorage.removeItem("username");
			state.token = null;
			state.username = "";
			state.isAuthenticated = false;
			state.error = null;
			state.successMessage = null;
		},
		clearMessages(state) {
			state.error = null;
			state.successMessage = null;
		},
	},
	extraReducers: (builder) => {
		// LOGIN
		builder
			.addCase(loginUser.pending, (state) => {
				state.loading = true;
				state.error = null;
				state.successMessage = null;
			})
			.addCase(loginUser.fulfilled, (state, action) => {
				const { token, username } = action.payload;
				state.token = token;
				state.username = username;
				state.isAuthenticated = true;
				state.loading = false;
			})
			.addCase(loginUser.rejected, (state, action) => {
				state.loading = false;
				state.error = Array.isArray(action.payload)
					? action.payload.join(", ")
					: action.payload || "Login failed.";
			});

		// REGISTER
		builder
			.addCase(registerUser.pending, (state) => {
				state.loading = true;
				state.error = null;
				state.successMessage = null;
			})
			.addCase(registerUser.fulfilled, (state, action) => {
				state.loading = false;
				state.successMessage =
					action.payload.message || "Registration successful!";
			})
			.addCase(registerUser.rejected, (state, action) => {
				state.loading = false;
				state.error = Array.isArray(action.payload)
					? action.payload.join(", ")
					: action.payload || "Registration failed.";
			});
	},
});

export const { logout, clearMessages } = authSlice.actions;
export default authSlice.reducer;
