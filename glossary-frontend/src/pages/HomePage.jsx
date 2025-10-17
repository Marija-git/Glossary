import GlossaryList from "../components/GlossaryTermsView";
import GlossaryCard from "../components/GlossaryCard";
import GlossaryPagination from "../components/GlossaryPagination";
import GlossaryNavbar from "../components/GlossaryNavbar";
import { useSelector, useDispatch } from "react-redux";
import { toast } from "react-toastify";
import { useEffect } from "react";
import { setPageIndex, fetchGlossaryTerms } from "../store/GlossaryTermSlice";

const HomePage = () => {
	const token = useSelector((state) => state.auth.token);
	const dispatch = useDispatch();
	const { glossaryTerms, loading, error, pageIndex, pageSize, totalPages } =
		useSelector((state) => state.glossaryTerms);

	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);

	const handlePageChange = (newPageIndex) => {
		dispatch(setPageIndex(newPageIndex));
	};

	useEffect(() => {
		if (token) {
			dispatch(fetchGlossaryTerms({ pageIndex, pageSize, token }));
		} else {
			dispatch(fetchGlossaryTerms({ pageIndex, pageSize }));
		}
	}, [dispatch, pageIndex, pageSize, token]);

	useEffect(() => {
		if (error) {
			toast.error(error);
		}
	}, [error]);

	return (
		<>
			<GlossaryNavbar />
			<div className='container mt-4'>
				{isLoggedIn && <GlossaryCard />}
				{loading && <p>Loading terms...</p>}
				<GlossaryList glossaryTerms={glossaryTerms} />
				<GlossaryPagination
					pageIndex={pageIndex}
					pageSize={pageSize}
					totalPages={totalPages}
					onPageChange={handlePageChange}
				/>
			</div>
		</>
	);
};

export default HomePage;
