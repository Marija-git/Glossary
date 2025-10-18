import { useState } from "react";
import { Table } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";
import ActionsColumn from "./ActionsColumn";
import { useSelector, useDispatch } from "react-redux";
import {
	publishGlossaryTermThunk,
	fetchGlossaryTerms,
} from "../store/GlossaryTermSlice";
import { toast } from "react-toastify";

const GlossaryTermsView = ({ glossaryTerms }) => {
	const dispatch = useDispatch();
	const token = useSelector((state) => state.auth.token);
	const [isModalOpen, setIsModalOpen] = useState(false);
	const [selectedglossaryTerm, setSelectedglossaryTerm] = useState(null);
	const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);

	const handlePublishClick = (term) => {
		setSelectedglossaryTerm(term);
		setIsModalOpen(true);
	};

	const handlePublish = async (updatedData) => {
		console.log("hello");
		if (!selectedglossaryTerm) return;

		const { term, definition } = updatedData;
		const { id } = selectedglossaryTerm;

		const resultAction = await dispatch(
			publishGlossaryTermThunk({ id, term, definition, token })
		);

		if (publishGlossaryTermThunk.fulfilled.match(resultAction)) {
			toast.success(resultAction.payload);
			dispatch(fetchGlossaryTerms({ pageIndex: 1, pageSize: 3, token }));
			setIsModalOpen(false);
		} else {
			toast.error(resultAction.payload);
		}
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
										item={term}
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
				initialData={selectedglossaryTerm}
			/>
		</div>
	);
};

export default GlossaryTermsView;
