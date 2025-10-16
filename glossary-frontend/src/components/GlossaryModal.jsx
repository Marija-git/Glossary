import { useState, useEffect } from "react";
import { Button, Form, Modal } from "react-bootstrap";

const GlossaryModal = ({
	show,
	onClose,
	onSubmit,
	mode = "create",
	initialData,
}) => {
	const [formData, setFormData] = useState({ term: "", definition: "" });

	useEffect(() => {
		if (initialData) {
			setFormData({
				term: initialData.term || "",
				definition: initialData.definition || "",
			});
		} else {
			setFormData({ term: "", definition: "" });
		}
	}, [initialData, show]);

	const handleChange = (e) => {
		setFormData({ ...formData, [e.target.name]: e.target.value });
	};

	const handleReset = () => {
		setFormData({ term: "", definition: "" });
	};

	const handleSubmit = (e) => {
		e.preventDefault();
		onSubmit(formData);
		onClose();
		handleReset();
	};

	return (
		<Modal
			show={show}
			onHide={onClose}
			centered>
			<Modal.Header closeButton>
				<Modal.Title>
					{mode === "create" ? "Create New Term" : "Publish Term"}
				</Modal.Title>
			</Modal.Header>

			<Modal.Body>
				<Form onSubmit={handleSubmit}>
					<Form.Group className='mb-3'>
						<Form.Label>Term</Form.Label>
						<Form.Control
							type='text'
							name='term'
							value={formData.term}
							onChange={handleChange}
							placeholder='Enter term'
							required
						/>
					</Form.Group>

					<Form.Group className='mb-3'>
						<Form.Label>Definition</Form.Label>
						<Form.Control
							as='textarea'
							rows={3}
							name='definition'
							value={formData.definition}
							onChange={handleChange}
							placeholder='Enter definition'
							required
						/>
					</Form.Group>

					<div className='d-flex justify-content-end gap-2'>
						<Button
							variant='secondary'
							onClick={handleReset}>
							Reset
						</Button>
						<Button
							variant='primary'
							type='submit'>
							{mode === "create" ? "Create" : "Confirm"}
						</Button>
					</div>
				</Form>
			</Modal.Body>
		</Modal>
	);
};

export default GlossaryModal;
