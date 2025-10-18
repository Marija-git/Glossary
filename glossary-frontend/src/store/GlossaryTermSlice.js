import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import {
	getPaginatedGlossaryTerms,
	createGlossaryTerm,
	publishGlossaryTerm,
} from "../services/GlossaryTermService";

export const fetchGlossaryTerms = createAsyncThunk(
	"glossary/fetchGlossaryTerms",
	async ({ pageSize = 10, pageIndex = 1, token = null }, thunkAPI) => {
		const { data, errors } = await getPaginatedGlossaryTerms(
			pageSize,
			pageIndex,
			token
		);
		if (errors) return thunkAPI.rejectWithValue(errors);
		return data;
	}
);

export const createGlossaryTermThunk = createAsyncThunk(
	"glossaryTerms/createGlossaryTerm",
	async ({ term, definition, token }, thunkAPI) => {
		const result = await createGlossaryTerm(term, definition, token);
		if (result.errors) return thunkAPI.rejectWithValue(result.errors);
		return result.message;
	}
);

export const publishGlossaryTermThunk = createAsyncThunk(
	"glossary/publishGlossaryTerm",
	async ({ id, term, definition, token }, thunkAPI) => {
		const result = await publishGlossaryTerm(id, term, definition, token);
		if (result.errors) return thunkAPI.rejectWithValue(result.errors);
		return result.message;
	}
);

const initialState = {
	glossaryTerms: [],
	loading: false,
	error: null,
	pageIndex: 1,
	pageSize: 3,
	totalPages: 0,
	totalCount: 0,
};

const glossaryTermsSlice = createSlice({
	name: "glossaryTerms",
	initialState,
	reducers: {
		clearGlossaryError(state) {
			state.error = null;
		},
		setPageIndex(state, action) {
			state.pageIndex = action.payload;
		},
		resetGlossary(state) {
			state.glossaryTerms = [];
			state.pageIndex = 1;
			state.totalPages = 0;
			state.totalCount = 0;
			state.loading = false;
			state.error = null;
		},
	},
	extraReducers: (builder) => {
		builder
			// fetch
			.addCase(fetchGlossaryTerms.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchGlossaryTerms.fulfilled, (state, action) => {
				const payload = action.payload || {};
				state.glossaryTerms = payload.items || [];
				state.pageIndex = payload.pageIndex || 1;
				state.totalPages = payload.totalPages || 0;
				state.totalCount = payload.totalCount || 0;
				state.loading = false;
			})
			.addCase(fetchGlossaryTerms.rejected, (state, action) => {
				state.loading = false;
				state.error = Array.isArray(action.payload)
					? action.payload.join(", ")
					: action.payload || "Failed to fetch glossary terms.";
			})

			//create
			.addCase(createGlossaryTermThunk.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(createGlossaryTermThunk.fulfilled, (state, action) => {
				state.loading = false;
				state.error = null;
			})
			.addCase(createGlossaryTermThunk.rejected, (state, action) => {
				state.loading = false;
				state.error = Array.isArray(action.payload)
					? action.payload.join(", ")
					: action.payload || "Failed to create glossary term.";
			})

			// publish
			.addCase(publishGlossaryTermThunk.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(publishGlossaryTermThunk.fulfilled, (state, action) => {
				state.loading = false;
				state.error = null;
			})
			.addCase(publishGlossaryTermThunk.rejected, (state, action) => {
				state.loading = false;
				state.error = Array.isArray(action.payload)
					? action.payload.join(", ")
					: action.payload || "Failed to publish glossary term.";
			});
	},
});

export const { clearGlossaryError, setPageIndex, resetGlossary } =
	glossaryTermsSlice.actions;
export default glossaryTermsSlice.reducer;
