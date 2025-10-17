import axios from "axios";
import { handleApiErrors } from "./errorHandler";
const BASE_URL = "https://localhost:7163/api/GlossaryTerms";

export const createGlossaryTerm = async (term, definition, token) => {
	try {
		const response = await axios.post(
			BASE_URL,
			{ Term: term, Definition: definition },
			{
				headers: {
					Authorization: `Bearer ${token}`,
					"Content-Type": "application/json",
				},
			}
		);
		return { message: "Glossary term successfully created!", errors: null };
	} catch (error) {
		return { message: null, errors: handleApiErrors(error) };
	}
};

export const deleteGlossaryTerm = async (id, token) => {
	try {
		await axios.delete(`${BASE_URL}/${id}`, {
			headers: {
				Authorization: `Bearer ${token}`,
			},
		});
		return { message: "Glossary term successfully deleted!", errors: null };
	} catch (error) {
		return { message: null, errors: handleApiErrors(error) };
	}
};

export const archiveGlossaryTerm = async (id, token) => {
	try {
		await axios.patch(`${BASE_URL}/${id}/archive`, null, {
			headers: {
				Authorization: `Bearer ${token}`,
			},
		});
		return { message: "Glossary term successfully archived!", errors: null };
	} catch (error) {
		return { message: null, errors: handleApiErrors(error) };
	}
};

export const publishGlossaryTerm = async (id, term, definition, token) => {
	try {
		await axios.patch(
			`${BASE_URL}/${id}/publish`,
			{ Term: term, Definition: definition },
			{
				headers: {
					Authorization: `Bearer ${token}`,
					"Content-Type": "application/json",
				},
			}
		);
		return { message: "Glossary term successfully published!", errors: null };
	} catch (error) {
		return { message: null, errors: handleApiErrors(error) };
	}
};

export const getPaginatedGlossaryTerms = async (
	pageSize = 10,
	pageIndex = 1,
	token = null
) => {
	try {
		const response = await axios.get(`${BASE_URL}/paginated-data`, {
			params: { pageSize, pageIndex },
			headers: token
				? {
						Authorization: `Bearer ${token}`,
				  }
				: {},
		});

		return { data: response.data, errors: null };
	} catch (error) {
		return { data: null, errors: handleApiErrors(error) };
	}
};
