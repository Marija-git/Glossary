import { useState } from "react";
import { Button, Card } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";
import { toast } from "react-toastify";
import { useSelector, useDispatch } from "react-redux";
import {
	fetchGlossaryTerms,
	createGlossaryTermThunk,
} from "../store/GlossaryTermSlice";

const GlossaryCard = () => {
	const [isModalOpen, setIsModalOpen] = useState(false);
	const token = useSelector((state) => state.auth.token);
	const dispatch = useDispatch();

	const handleCreate = async (data) => {
		const { term, definition } = data;

		const resultAction = await dispatch(
			createGlossaryTermThunk({ term, definition, token })
		);
		if (createGlossaryTermThunk.fulfilled.match(resultAction)) {
			toast.success(resultAction.payload);
			dispatch(fetchGlossaryTerms({ pageIndex: 1, pageSize: 3, token }));
		} else {
			toast.error(resultAction.payload);
		}
	};
	return (
		<>
			<Card
				className='mb-4 text-center mx-auto'
				style={{ maxWidth: "600px" }}>
				<Card.Body>
					<Card.Text>You can add a new term here</Card.Text>
					<Button onClick={() => setIsModalOpen(true)}>Create New Term</Button>
				</Card.Body>
			</Card>
			<GlossaryModal
				show={isModalOpen}
				onClose={() => setIsModalOpen(false)}
				onSubmit={handleCreate}
				mode='create'
			/>
		</>
	);
};

export default GlossaryCard;
