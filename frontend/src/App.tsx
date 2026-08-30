import { useState, ChangeEvent, SubmitEvent } from 'react'
import { Plus, CheckCircle2, Circle, ListTodo } from 'lucide-react'

// Importaciones desde la carpeta shared usando el alias
import { Button } from '@shared/components/ui/button'
import { Input } from '@shared/components/ui/input'
import { Badge } from '@shared/components/ui/badge'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@shared/components/ui/card'

interface Task {
  id: number
  title: string
  completed: boolean
}

export default function App() {
  const [count, setCount] = useState<number>(0)
  const [tasks, setTasks] = useState<Task[]>([
    { id: 1, title: 'Conectar Frontend React a la API .NET', completed: false },
    { id: 2, title: 'Configurar shadcn/ui y Tailwind v4', completed: true },
  ])
  const [newTaskTitle, setNewTaskTitle] = useState<string>('')

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    setNewTaskTitle(e.target.value)
  }

  const handleAddTask = (e: SubmitEvent<HTMLFormElement>) => {
    e.preventDefault()
    if (!newTaskTitle.trim()) return

    const newTask: Task = {
      id: Date.now(),
      title: newTaskTitle,
      completed: false,
    }

    setTasks((prevTasks) => [...prevTasks, newTask])
    setNewTaskTitle('')
  }

  const toggleTask = (id: number) => {
    setTasks((prevTasks) =>
      prevTasks.map((task) =>
        task.id === id ? { ...task, completed: !task.completed } : task
      )
    )
  }

  return (
    <div className="min-h-screen bg-background text-foreground flex flex-col items-center justify-center p-6">
      <Card className="max-w-md w-full shadow-lg border-border">
        <CardHeader className="text-center space-y-2">
          <div className="flex justify-center items-center gap-2">
            <ListTodo className="h-6 w-6 text-primary" />
            <CardTitle className="text-2xl font-bold">React + shadcn/ui</CardTitle>
          </div>
          <CardDescription>
            Demostración de componentes shadcn/ui y TypeScript estricto.
          </CardDescription>

          <div className="pt-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCount((prev) => prev + 1)}
            >
              Contador: {count}
            </Button>
          </div>
        </CardHeader>

        <CardContent className="space-y-6">
          {/* Formulario usando Input y Button de shadcn */}
          <form onSubmit={handleAddTask} className="flex gap-2">
            <Input
              type="text"
              value={newTaskTitle}
              onChange={handleInputChange}
              placeholder="Nueva tarea..."
            />
            <Button type="submit" size="icon" aria-label="Agregar tarea">
              <Plus className="h-4 w-4" />
            </Button>
          </form>

          {/* Lista de tareas con Badge e Iconos */}
          <ul className="space-y-2">
            {tasks.map((task: Task) => (
              <li
                key={task.id}
                onClick={() => toggleTask(task.id)}
                className="flex items-center justify-between p-3 rounded-lg border border-border bg-card hover:bg-accent/50 cursor-pointer transition-colors"
              >
                <div className="flex items-center gap-2">
                  {task.completed ? (
                    <CheckCircle2 className="h-4 w-4 text-emerald-500" />
                  ) : (
                    <Circle className="h-4 w-4 text-muted-foreground" />
                  )}
                  <span
                    className={
                      task.completed
                        ? 'line-through text-muted-foreground'
                        : 'font-medium'
                    }
                  >
                    {task.title}
                  </span>
                </div>

                <Badge variant={task.completed ? 'default' : 'secondary'}>
                  {task.completed ? 'Hecho' : 'Pendiente'}
                </Badge>
              </li>
            ))}
          </ul>
        </CardContent>
      </Card>
    </div>
  )
}