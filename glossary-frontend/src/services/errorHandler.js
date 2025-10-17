export function handleApiErrors(error) {
	//axios
	if (error.response) {
		const data = error.response.data;

		//data annotations
		if (data.errors) return Object.values(data.errors).flat();

		//custom exception middleware
		if (data.error) return [data.error];

		//general message
		return [data.message || "An unexpected error occurred."];
	}

	//network
	if (error.request) {
		return ["Network error. Please check your connection."];
	}

	//another
	return [error.message || "An unexpected error occurred."];
}
