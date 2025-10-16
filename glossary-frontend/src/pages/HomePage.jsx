import GlossaryList from "../components/GlossaryTermsView";
import GlossaryCard from "../components/GlossaryCard";
import GlossaryPagination from "../components/GlossaryPagination";
import GlossaryNavbar from "../components/GlossaryNavbar";
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
	const isLoggedIn = true;
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
