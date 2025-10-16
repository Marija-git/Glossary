import { useState } from "react";
import { Button, Card } from "react-bootstrap";
import GlossaryModal from "./GlossaryModal";

const GlossaryCard = () => {
	const [isModalOpen, setIsModalOpen] = useState(false);

	const handleCreate = (data) => {
		console.log("Created term:", data);
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
