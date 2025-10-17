import { useState } from "react";
import { Table } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";
import ActionsColumn from "./ActionsColumn";
import { useSelector } from "react-redux";

const GlossaryList = ({ glossaryTerms }) => {
	const [isModalOpen, setIsModalOpen] = useState(false);
	const [selectedglossaryTerms, setSelectedglossaryTerms] = useState(null);
	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);

	const handlePublishClick = (term) => {
		setSelectedglossaryTerms(term);
		setIsModalOpen(true);
	};

	const handlePublish = (updatedData) => {
		console.log("Published/Updated term:", updatedData);
		setIsModalOpen(false);
	};
	return (
		<div className='p-4'>
			<div className='table-responsive'>
				<Table
					hover
					className='align-middle text-center shadow-sm rounded'>
					<thead className='table-primary'>
						<tr>
							<th>Term</th>
							<th>Definition</th>
							<th>Status</th>
							{isLoggedIn && <th>Actions</th>}
						</tr>
					</thead>
					<tbody>
						{glossaryTerms.map((term) => (
							<tr key={term.id}>
								<td>{term.term}</td>
								<td>{term.definition}</td>
								<td>{term.status}</td>
								{isLoggedIn && (
									<ActionsColumn
										glossaryTerms={term}
										onPublishClick={handlePublishClick}
										onDelete={(term) => console.log("Deleted:", term)}
										onArchive={(term) => console.log("Archived:", term)}
									/>
								)}
							</tr>
						))}
					</tbody>
				</Table>
			</div>
			<GlossaryModal
				show={isModalOpen}
				onClose={() => setIsModalOpen(false)}
				onSubmit={handlePublish}
				mode='update'
				initialData={selectedglossaryTerms}
			/>
		</div>
	);
};

export default GlossaryList;
