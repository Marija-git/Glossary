import { useState } from "react";
import { Table } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";
import ActionsColumn from "./ActionsColumn";
import { useSelector } from "react-redux";

const GlossaryList = ({ items }) => {
	const [isModalOpen, setIsModalOpen] = useState(false);
	const [selectedItem, setSelectedItem] = useState(null);
	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);

	const handlePublishClick = (item) => {
		setSelectedItem(item);
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
						{items.map((item) => (
							<tr key={item.term}>
								<td>{item.term}</td>
								<td>{item.definition}</td>
								<td>{item.status}</td>
								{isLoggedIn && (
									<ActionsColumn
										item={item}
										onPublishClick={handlePublishClick}
										onDelete={(item) => console.log("Deleted:", item)}
										onArchive={(item) => console.log("Archived:", item)}
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
				initialData={selectedItem}
			/>
		</div>
	);
};

export default GlossaryList;
