import { useState } from "react";
import { Table } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";
import ActionsColumn from "./ActionsColumn";
import { useSelector, useDispatch } from "react-redux";
import {
	publishGlossaryTermThunk,
	fetchGlossaryTerms,
	deleteGlossaryTermThunk,
	archiveGlossaryTermThunk,
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

	const handleDelete = async (term) => {
		if (!window.confirm(`Are you sure you want to delete "${term.term}"?`))
			return;

		const resultAction = await dispatch(
			deleteGlossaryTermThunk({ id: term.id, token })
		);

		if (deleteGlossaryTermThunk.fulfilled.match(resultAction)) {
			toast.success(resultAction.payload);
			dispatch(fetchGlossaryTerms({ pageIndex: 1, pageSize: 3, token }));
		} else {
			toast.error(resultAction.payload);
		}
	};
	const handleArchive = async (term) => {
		if (!window.confirm(`Are you sure you want to archive "${term.term}"?`))
			return;

		const resultAction = await dispatch(
			archiveGlossaryTermThunk({ id: term.id, token })
		);

		if (archiveGlossaryTermThunk.fulfilled.match(resultAction)) {
			toast.success(resultAction.payload);
			dispatch(fetchGlossaryTerms({ pageIndex: 1, pageSize: 3, token }));
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
										onDelete={handleDelete}
										onArchive={handleArchive}
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
