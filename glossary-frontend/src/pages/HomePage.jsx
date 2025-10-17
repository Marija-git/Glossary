import GlossaryList from "../components/GlossaryTermsView";
import GlossaryCard from "../components/GlossaryCard";
import GlossaryPagination from "../components/GlossaryPagination";
import GlossaryNavbar from "../components/GlossaryNavbar";
import { useSelector } from "react-redux";
const mockItems = [
	{
		term: "API",
		definition: "Application Programming Interface",
		status: "Published",
	},
	{ term: "Component", definition: "Reusable piece of UI", status: "Draft" },
	{ term: "Hook", definition: "Special React function", status: "Archived" },
];

const HomePage = () => {
	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);
	return (
		<>
			<GlossaryNavbar />
			<div className='container mt-4'>
				{isLoggedIn && <GlossaryCard />}
				<GlossaryList items={mockItems} />
				<GlossaryPagination />
			</div>
		</>
	);
};

export default HomePage;
