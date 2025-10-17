import { configureStore } from "@reduxjs/toolkit";
import authReducer from "./AuthSlice";
import glossaryTermsReducer from "./GlossaryTermSlice";

const store = configureStore({
	reducer: {
		auth: authReducer,
		glossaryTerms: glossaryTermsReducer,
	},
});

export default store;
